using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Question;
using SDLS.Model.DTOs.QuestionTag;
using SDLS.Model.Models;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(
            IQuestionRepository questionRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }


        public async Task<PagedResult<QuestionDTO>> GetAllAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? QuestionCategoryId = null,
            List<Guid>? tagIds = null,
            string? searchContent = null,
            int? status = null,
            int page = 1,
            int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var filteredQuestions = await _questionRepository.GetFilteredForListAsync(
                lessonId,
                topicId,
                QuestionCategoryId,
                tagIds,
                searchContent,
                status,
                role);

            var orderedList = BuildOrderedLinkedList(filteredQuestions);
            var total = orderedList.Count;

            var pagedEntities = orderedList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedDtos = _mapper.Map<List<QuestionDTO>>(pagedEntities);

            for (int i = 0; i < pagedDtos.Count; i++)
            {
                pagedDtos[i].Position = (page - 1) * pageSize + i + 1;
            }

            return new PagedResult<QuestionDTO>
            {
                Items = pagedDtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }


        public async Task<QuestionDTO> GetByIdAsync(Guid id)
        {
            var role = UserContextHelper.GetRole(_httpContextAccessor);

            var question = await _questionRepository.GetByIdAsync(id, role);
            if (question == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<QuestionDTO>(question);
        }


        public async Task<bool> CreateAsync(QuestionCreateDTO dto)
        {
            if (dto.Answers == null || !dto.Answers.Any())
                throw new ArgumentException("Question must have at least 1 answer");

            if (!dto.Answers.Any(a => a.Iscorrect))
                throw new ArgumentException("At least one answer must be correct");

            var now = DateTime.UtcNow.ToLocalTime();

            var newQuestion = _mapper.Map<Question>(dto);
            newQuestion.Id = Guid.NewGuid();
            newQuestion.CreateAt = now;
            newQuestion.UpdateAt = now;
            newQuestion.Status = 1;
            newQuestion.Image = dto.Image;

            if (dto.Index.HasValue)
            {
                newQuestion.Index = dto.Index.Value;
            }
            else
            {
                var allActive = await _questionRepository.GetAllAsync(status: 1);
                var maxIndex = allActive.Max(q => q.Index ?? 0);
                newQuestion.Index = maxIndex + 1;
            }

            foreach (var ans in newQuestion.Answers)
            {
                ans.QuestionId = newQuestion.Id;
                ans.CreateAt = now;
                ans.UpdateAt = now;
                ans.Status = 1;
            }

            foreach (var questionTag in newQuestion.QuestionTags)
            {
                questionTag.QuestionId = newQuestion.Id;
                questionTag.CreateAt = now;
                questionTag.UpdateAt = now;
                questionTag.Status = 1;
            }

            newQuestion.ParentId = null;

            await _questionRepository.AddAsync(newQuestion);
            await RebuildGlobalParentLinksAsync(now);
            return true;
        }

        public async Task<bool> CreateManyAsync(List<QuestionCreateDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                throw new ArgumentException("Danh sách câu hỏi không được rỗng.");

            await using var transaction = await _questionRepository.BeginTransactionAsync();
            try
            {
                foreach (var dto in dtos)
                {
                    await CreateAsync(dto);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionUpdateDTO dto)
        {
            var existing = await _questionRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy câu hỏi");

            var now = DateTime.UtcNow.ToLocalTime();

            existing.QuestionLessonId = dto.QuestionLessonId;
            existing.QuestionTopicId = dto.QuestionTopicId;
            existing.QuestionCategoryId = dto.QuestionCategoryId;
            existing.Index = dto.Index ?? existing.Index;
            existing.Content = dto.Content;
            existing.Image = dto.Image;
            existing.Explanation = dto.Explanation;
            existing.Type = dto.Type;
            existing.UpdateAt = now;

            if (dto.Answers != null)
            {
                var existingAnswersById = existing.Answers.ToDictionary(a => a.Id, a => a);

                foreach (var answerDto in dto.Answers)
                {
                    if (answerDto.QuestionId != id)
                        throw new ArgumentException($"Answer.QuestionId ({answerDto.QuestionId}) không khớp Question Id ({id}).");

                    if (answerDto.Id.HasValue)
                    {
                        if (!existingAnswersById.TryGetValue(answerDto.Id.Value, out var answer))
                            throw new KeyNotFoundException($"Không tìm thấy Answer với Id {answerDto.Id.Value}");

                        answer.Content = answerDto.Content;
                        answer.IsCorrect = answerDto.Iscorrect;
                        answer.UpdateAt = now;
                        answer.Status = answerDto.Status ?? answer.Status ?? 1;
                    }
                    else
                    {
                        var newAnswer = new Answer
                        {
                            QuestionId = id,
                            Content = answerDto.Content,
                            IsCorrect = answerDto.Iscorrect,
                            CreateAt = now,
                            UpdateAt = now,
                            Status = answerDto.Status ?? 1
                        };

                        existing.Answers.Add(newAnswer);
                    }
                }
            }

            if (dto.QuestionTags != null)
            {
                // 1) Hard delete toàn bộ tag cũ của question
                _questionRepository.RemoveQuestionTags(existing.QuestionTags.ToList());

                // 2) Thêm lại tag mới (distinct để tránh trùng unique questionId + tagId)
                var newTagIds = dto.QuestionTags
                    .Select(qt => qt.TagId)
                    .Where(tagId => tagId != Guid.Empty)
                    .Distinct()
                    .ToList();

                foreach (var tagId in newTagIds)
                {
                    var newQuestionTag = new QuestionTag
                    {
                        QuestionId = id,
                        TagId = tagId,
                        CreateAt = now,
                        UpdateAt = now,
                        Status = 1
                    };

                    existing.QuestionTags.Add(newQuestionTag);
                }
            }

            await _questionRepository.UpdateAsync(existing);
            await RebuildGlobalParentLinksAsync(now);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await DeleteSoftAsync(id);
        }

        public async Task<bool> DeleteSoftAsync(Guid id)
        {
            var existing = await _questionRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Không tìm thấy câu hỏi với Id {id}");

            var now = DateTime.UtcNow.ToLocalTime();

            existing.Status = 0;
            existing.UpdateAt = now;
            existing.ParentId = null;

            await _questionRepository.UpdateAsync(existing);
            await RebuildGlobalParentLinksAsync(now);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _questionRepository.DeleteHardAsync(id);
            return true;
        }


        


        // private method

        private List<Question> BuildOrderedLinkedList(IEnumerable<Question> all)
        {
            return all
                .OrderBy(q => q.Index ?? int.MaxValue)
                .ThenBy(q => q.CreateAt ?? DateTime.MinValue)
                .ThenBy(q => q.Id)
                .ToList();
        }

        private async Task RebuildGlobalParentLinksAsync(DateTime now)
        {
            var allActive = await _questionRepository.GetAllAsync(status: 1);
            var ordered = allActive
                .OrderBy(q => q.Index ?? int.MaxValue)
                .ThenBy(q => q.CreateAt ?? DateTime.MinValue)
                .ThenBy(q => q.Id)
                .ToList();

            if (!ordered.Any())
                return;

            var changedTracked = new List<Question>();

            for (var i = 0; i < ordered.Count; i++)
            {
                var currentId = ordered[i].Id;
                var expectedParentId = i + 1 < ordered.Count ? ordered[i + 1].Id : (Guid?)null;

                if (ordered[i].ParentId == expectedParentId)
                    continue;

                var tracked = await _questionRepository.GetByIdForUpdateAsync(currentId);
                if (tracked == null)
                    continue;

                tracked.ParentId = expectedParentId;
                tracked.UpdateAt = now;
                changedTracked.Add(tracked);
            }

            if (changedTracked.Any())
            {
                await _questionRepository.UpdateAsync(changedTracked[0]);
            }
        }

        private static QuestionCreateDTO BuildQuestionCreateDto(
            Dictionary<string, string> row,
            int rowNo,
            IReadOnlyDictionary<string, Guid> lessonLookup,
            IReadOnlyDictionary<string, Guid> topicLookup,
            IReadOnlyDictionary<string, Guid> categoryLookup,
            IReadOnlyDictionary<string, Guid> tagLookup)
        {
            var importRow = new QuestionImportRowDTO
            {
                QuestionLessonName = GetRequired(row, "QuestionLessonName", rowNo),
                QuestionTopicName = GetRequired(row, "QuestionTopicName", rowNo),
                QuestionCategoryName = GetRequired(row, "QuestionCategoryName", rowNo),
                Index = ParseIntOptional(row, "Index"),
                Content = GetRequired(row, "Content", rowNo),
                Image = GetOptional(row, "Image"),
                Explanation = GetOptional(row, "Explanation"),
                Type = GetOptional(row, "Type"),
                Answers = GetRequired(row, "Answers", rowNo),
                QuestionTagNames = GetOptional(row, "QuestionTagNames")
            };

            var lessonKey = NormalizeLookupKey(importRow.QuestionLessonName);
            if (!lessonLookup.TryGetValue(lessonKey, out var lessonId))
                throw new ArgumentException($"Không tìm thấy QuestionLesson theo tên: '{importRow.QuestionLessonName}'.");

            var topicKey = NormalizeLookupKey(importRow.QuestionTopicName);
            if (!topicLookup.TryGetValue(topicKey, out var topicId))
                throw new ArgumentException($"Không tìm thấy QuestionTopic theo tên: '{importRow.QuestionTopicName}'.");

            var categoryKey = NormalizeLookupKey(importRow.QuestionCategoryName);
            if (!categoryLookup.TryGetValue(categoryKey, out var categoryId))
                throw new ArgumentException($"Không tìm thấy QuestionCategory theo tên: '{importRow.QuestionCategoryName}'.");

            var dto = new QuestionCreateDTO
            {
                QuestionLessonId = lessonId,
                QuestionTopicId = topicId,
                QuestionCategoryId = categoryId,
                Index = importRow.Index,
                Content = importRow.Content,
                Image = importRow.Image,
                Explanation = importRow.Explanation,
                Type = importRow.Type,
                Answers = ParseAnswers(importRow.Answers),
                QuestionTags = ParseQuestionTags(importRow.QuestionTagNames, tagLookup)
            };

            return dto;
        }

        private static List<AnswerCreateDTO> ParseAnswers(string raw)
        {
            var result = new List<AnswerCreateDTO>();

            foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var pieces = part.Split('|', 2, StringSplitOptions.TrimEntries);
                if (pieces.Length != 2)
                    throw new ArgumentException("Cột Answers sai định dạng. Dùng: Content|true;Content|false");

                if (!bool.TryParse(pieces[1], out var isCorrect))
                    throw new ArgumentException("IsCorrect trong cột Answers phải là true/false.");

                result.Add(new AnswerCreateDTO
                {
                    Content = pieces[0],
                    Iscorrect = isCorrect
                });
            }

            return result;
        }

        private static List<QuestionTagCreateDTO> ParseQuestionTags(string? raw, IReadOnlyDictionary<string, Guid> tagLookup)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<QuestionTagCreateDTO>();

            var tags = new List<QuestionTagCreateDTO>();
            foreach (var part in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var key = NormalizeLookupKey(part);
                if (!tagLookup.TryGetValue(key, out var tagId))
                    throw new ArgumentException($"Không tìm thấy Tag theo tên: '{part}'.");

                tags.Add(new QuestionTagCreateDTO { TagId = tagId });
            }

            return tags;
        }

        private static int? ParseIntOptional(Dictionary<string, string> row, string key)
        {
            var raw = GetOptional(row, key);
            if (string.IsNullOrWhiteSpace(raw)) return null;
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : null;
        }

        private static string GetRequired(Dictionary<string, string> row, string key, int rowNo)
        {
            if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Thiếu cột bắt buộc {key}.");
            return value.Trim();
        }

        private static string? GetOptional(Dictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out var value) ? value?.Trim() : null;
        }

        private static string NormalizeLookupKey(string? value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}
