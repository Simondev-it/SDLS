using AutoMapper;
using Microsoft.AspNetCore.Http;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Question;
using SDLS.Model.Models;
using SDLS.Model.Enumerations;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(
            IQuestionRepository questionRepository,
            IStorageService storageService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _storageService = storageService;
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

            newQuestion.ParentId = nextId;

            if (prevId.HasValue)
            {
                var prevTracked = await _questionRepository.GetByIdForUpdateAsync(prevId.Value);
                if (prevTracked == null)
                    throw new KeyNotFoundException($"Previous question not found: {prevId.Value}");

                prevTracked.ParentId = newQuestion.Id;
                prevTracked.UpdateAt = now;
            }

            // trong CreateAsync, sau khi newQuestion.Id = Guid.NewGuid();
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                newQuestion.Image = await _storageService.UploadImageAsync(dto.ImageFile, ImageTarget.QuestionImage, newQuestion.Id);
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
            existing.Content = dto.Content;
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

            // nếu có upload ảnh mới thì ghi đè ảnh cũ
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                existing.Image = await _storageService.UploadImageAsync(dto.ImageFile, ImageTarget.QuestionImage, id);
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
    }
}
