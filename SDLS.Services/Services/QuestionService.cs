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
        private readonly AppDbContext _dbContext;
        private readonly IImportCoreService _importCoreService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(
            IQuestionRepository questionRepository,
            AppDbContext dbContext,
            IImportCoreService importCoreService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _dbContext = dbContext;
            _importCoreService = importCoreService;
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

            var lessonQuestions = await _questionRepository.GetAllByLessonAsync(dto.QuestionLessonId);
            var ordered = BuildOrderedLinkedList(lessonQuestions);

            var position = NormalizePosition(dto.Position, ordered.Count);
            ResolveInsertNeighbors(ordered, position, out var prevId, out var nextId);

            newQuestion.Index = dto.Index ?? position;
            newQuestion.ParentId = nextId;

            if (prevId.HasValue)
            {
                var prevTracked = await _questionRepository.GetByIdForUpdateAsync(prevId.Value);
                if (prevTracked == null)
                    throw new KeyNotFoundException($"Previous question not found: {prevId.Value}");

                prevTracked.ParentId = newQuestion.Id;
                prevTracked.UpdateAt = now;
            }

            await _questionRepository.AddAsync(newQuestion);
            return true;
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
            existing.Index = dto.Index ?? dto.Position ?? existing.Index;
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

            // Reorder chỉ khi có Position
            if (dto.Position.HasValue)
            {
                var lessonQuestions = await _questionRepository.GetAllByLessonAsync(existing.QuestionLessonId);
                var ordered = BuildOrderedLinkedList(lessonQuestions);

                var currentIndex = ordered.FindIndex(q => q.Id == existing.Id);
                if (currentIndex < 0)
                    throw new InvalidOperationException("Question không tồn tại trong chuỗi linked list hiện tại.");

                // oldPrev là node đang trỏ tới existing
                var oldPrevId = ordered.FirstOrDefault(q => q.ParentId == existing.Id)?.Id;
                var oldNextId = existing.ParentId;

                // Gỡ existing khỏi vị trí cũ: oldPrev -> oldNext
                if (oldPrevId.HasValue)
                {
                    var oldPrevTracked = await _questionRepository.GetByIdForUpdateAsync(oldPrevId.Value);
                    if (oldPrevTracked != null)
                    {
                        oldPrevTracked.ParentId = oldNextId;
                        oldPrevTracked.UpdateAt = now;
                    }
                }

                // Remove existing khỏi list để tính vị trí mới
                ordered.RemoveAt(currentIndex);

                var newPosition = NormalizePosition(dto.Position, ordered.Count);
                ResolveInsertNeighbors(ordered, newPosition, out var newPrevId, out var newNextId);

                // existing -> newNext
                existing.ParentId = newNextId;

                // newPrev -> existing
                if (newPrevId.HasValue)
                {
                    var newPrevTracked = await _questionRepository.GetByIdForUpdateAsync(newPrevId.Value);
                    if (newPrevTracked != null)
                    {
                        newPrevTracked.ParentId = existing.Id;
                        newPrevTracked.UpdateAt = now;
                    }
                }
            }

            await _questionRepository.UpdateAsync(existing);
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

            var lessonQuestions = await _questionRepository.GetAllByLessonAsync(existing.QuestionLessonId);
            var prevId = lessonQuestions.FirstOrDefault(q => q.ParentId == existing.Id)?.Id;
            var nextId = existing.ParentId;

            if (prevId.HasValue)
            {
                var prevTracked = await _questionRepository.GetByIdForUpdateAsync(prevId.Value);
                if (prevTracked != null)
                {
                    prevTracked.ParentId = nextId;
                    prevTracked.UpdateAt = now;
                }
            }

            existing.Status = 0;
            existing.UpdateAt = now;
            existing.ParentId = null;

            await _questionRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteHardAsync(Guid id)
        {
            await _questionRepository.DeleteHardAsync(id);
            return true;
        }

        public async Task<byte[]> DownloadImportTemplateAsync(string format = "xlsx")
        {
            var headers = new[]
            {
                "QuestionLessonName",
                "QuestionTopicName",
                "QuestionCategoryName",
                "Position",
                "Content",
                "Image",
                "Explanation",
                "Type",
                "Answers",
                "QuestionTagNames"
            };

            var sample = new[]
            {
                "Bài 1: Khái niệm và quy tắc",
                "Biển báo giao thông",
                "Lý thuyết",
                "1",
                "Nội dung câu hỏi mẫu",
                "https://example.com/question-image.jpg",
                "Giải thích mẫu",
                "single-choice",
                "Đáp án A|true;Đáp án B|false;Đáp án C|false;Đáp án D|false",
                "Biển báo;Sa hình"
            };

            return await _importCoreService.BuildTemplateAsync(headers, sample, format, "QuestionsTemplate");
        }

        public async Task<QuestionImportResultDTO> ImportQuestionsAsync(IFormFile file)
        {
            var rows = await _importCoreService.ReadRowsAsync(file);

            var result = new QuestionImportResultDTO { TotalRows = rows.Count };

            var lessonMap = await _dbContext.QuestionLessons
                .AsNoTracking()
                .Where(x => x.Status != 0)
                .ToListAsync();

            var topicMap = await _dbContext.QuestionTopics
                .AsNoTracking()
                .Where(x => x.Status != 0)
                .ToListAsync();

            var categoryMap = await _dbContext.QuestionCategories
                .AsNoTracking()
                .Where(x => x.Status != 0)
                .ToListAsync();

            var tagMap = await _dbContext.Tags
                .AsNoTracking()
                .Where(x => x.Status != 0)
                .ToListAsync();

            var lessonLookup = lessonMap
                .GroupBy(x => NormalizeLookupKey(x.Name))
                .ToDictionary(g => g.Key, g => g.First().Id);

            var topicLookup = topicMap
                .GroupBy(x => NormalizeLookupKey(x.Name))
                .ToDictionary(g => g.Key, g => g.First().Id);

            var categoryLookup = categoryMap
                .GroupBy(x => NormalizeLookupKey(x.Name))
                .ToDictionary(g => g.Key, g => g.First().Id);

            var tagLookup = tagMap
                .GroupBy(x => NormalizeLookupKey(x.Name))
                .ToDictionary(g => g.Key, g => g.First().Id);

            for (var index = 0; index < rows.Count; index++)
            {
                var rowNo = index + 2;
                var row = rows[index];
                try
                {
                    var dto = BuildQuestionCreateDto(
                        row,
                        rowNo,
                        lessonLookup,
                        topicLookup,
                        categoryLookup,
                        tagLookup);

                    await CreateAsync(dto);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Row {rowNo}: {ex.Message}");
                }
            }

            return result;
        }


        // private method

        private static int NormalizePosition(int? inputPosition, int currentCount)
        {
            if (!inputPosition.HasValue)
                return currentCount + 1;

            if (inputPosition.Value < 1)
                return 1;

            if (inputPosition.Value > currentCount + 1)
                return currentCount + 1;

            return inputPosition.Value;
        }

        private static void ResolveInsertNeighbors(List<Question> ordered, int position, out Guid? prevId, out Guid? nextId)
        {
            prevId = null;
            nextId = null;

            if (ordered.Count == 0)
                return;

            if (position == 1)
            {
                nextId = ordered[0].Id;
                return;
            }

            if (position == ordered.Count + 1)
            {
                prevId = ordered[^1].Id;
                return;
            }

            prevId = ordered[position - 2].Id;
            nextId = ordered[position - 1].Id;
        }

        private List<Question> BuildOrderedLinkedList(IEnumerable<Question> all)
        {
            var allList = all.ToList();
            if (allList.Count == 0)
                return new List<Question>();

            var byId = allList.ToDictionary(q => q.Id, q => q);
            var referencedAsNext = allList
                .Where(q => q.ParentId.HasValue)
                .Select(q => q.ParentId!.Value)
                .ToHashSet();

            // Head: question không nằm trong ParentId của question khác
            var heads = allList
                .Where(q => !referencedAsNext.Contains(q.Id))
                .ToList();

            if (!heads.Any())
                heads.Add(allList[0]);

            var result = new List<Question>();
            var visited = new HashSet<Guid>();

            foreach (var head in heads)
            {
                var current = head;

                while (current != null && visited.Add(current.Id))
                {
                    result.Add(current);

                    if (current.ParentId.HasValue && byId.TryGetValue(current.ParentId.Value, out var next))
                        current = next;
                    else
                        current = null;
                }
            }

            // Nếu có cycle/disconnected node thì append nốt
            foreach (var q in allList)
            {
                if (!visited.Contains(q.Id))
                    result.Add(q);
            }

            return result;
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
                Position = ParseIntOptional(row, "Position"),
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
                Position = importRow.Position,
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
