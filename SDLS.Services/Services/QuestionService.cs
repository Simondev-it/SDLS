using AutoMapper;
using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Question;
using SDLS.Model.Models;
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
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }


        public async Task<PagedResult<QuestionDTO>> GetAllAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? QuestionCategoryId = null,
            int page = 1,
            int pageSize = 10)
        {
            var allQuestions = await _questionRepository.GetAllAsync();

            var orderedList = BuildOrderedLinkedList(allQuestions);

            if (lessonId.HasValue)
                orderedList = orderedList.Where(q => q.QuestionLessonId == lessonId.Value).ToList();

            if (topicId.HasValue)
                orderedList = orderedList.Where(q => q.QuestionTopicId == topicId.Value).ToList();

            if (QuestionCategoryId.HasValue)
                orderedList = orderedList.Where(q => q.QuestionCategoryId == QuestionCategoryId.Value).ToList();

            var total = orderedList.Count;

            // Lấy phần cần paginate (vẫn là entity)
            var pagedEntities = orderedList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map sang DTO
            var pagedDtos = _mapper.Map<List<QuestionDTO>>(pagedEntities);

            // Gán Position vào DTO (không động vào entity)
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
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
                throw new KeyNotFoundException($"Not found with ID {id}");

            return _mapper.Map<QuestionDTO>(question);
        }


        public async Task<bool> CreateAsync(QuestionCreateDTO dto)
        {
            // Validation
            if (dto.Answers == null || !dto.Answers.Any())
                throw new ArgumentException("Question must have at least 1 answer");

            if (!dto.Answers.Any(a => a.Iscorrect))
                throw new ArgumentException("At least one answer must be correct");

            // Map DTO → Entity
            var newQuestion = _mapper.Map<Question>(dto);
            newQuestion.Id = Guid.NewGuid();
            newQuestion.CreateAt = DateTime.UtcNow.ToLocalTime();
            newQuestion.UpdateAt = DateTime.UtcNow.ToLocalTime();
            newQuestion.Status = 1;

            // Xử lý Answers
            foreach (var ans in newQuestion.Answers)
            {
                ans.Id = Guid.NewGuid();
                ans.QuestionId = newQuestion.Id;
                ans.CreateAt = DateTime.UtcNow.ToLocalTime();
                ans.Status = 1;
            }

            // Lưu (EF sẽ insert Question + Answers cascade)
            await _questionRepository.AddAsync(newQuestion);

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionUpdateDTO dto)
        {
            var existing = await _questionRepository.GetByIdForUpdateAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Không tìm thấy câu hỏi");

            var now = DateTime.UtcNow.ToLocalTime();

            // Update thông tin Question
            existing.QuestionLessonId = dto.QuestionLessonId;
            existing.QuestionTopicId = dto.QuestionTopicId;
            existing.QuestionCategoryId = dto.QuestionCategoryId;
            existing.ParentId = dto.ParentId;
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
                    // Validate QuestionId truyền lên phải khớp question đang update
                    if (answerDto.QuestionId != id)
                        throw new ArgumentException($"Answer.QuestionId ({answerDto.QuestionId}) không khớp Question Id ({id}).");

                    if (answerDto.Id.HasValue)
                    {
                        // Có Id => chỉ update answer đã tồn tại
                        if (!existingAnswersById.TryGetValue(answerDto.Id.Value, out var answer))
                            throw new KeyNotFoundException($"Không tìm thấy Answer với Id {answerDto.Id.Value}");

                        if (answer.QuestionId != answerDto.QuestionId)
                            throw new ArgumentException($"Answer Id {answerDto.Id.Value} không thuộc Question {id}");

                        answer.Content = answerDto.Content;
                        answer.IsCorrect = answerDto.Iscorrect;
                        answer.UpdateAt = now;
                        answer.Status = answerDto.Status ?? answer.Status ?? 1;
                    }
                    else
                    {
                        // Id null => tạo answer mới
                        var newAnswer = new Answer
                        {
                            //Id = Guid.NewGuid(),
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

            await _questionRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _questionRepository.DeleteAsync(id);
            return true;
        }


        // private method

        private List<Question> BuildOrderedLinkedList(IEnumerable<Question> all)
        {
            var allList = all.ToList();
            var visited = new HashSet<Guid>();
            
            // Dùng Dictionary nhóm theo ParentId để tra cứu O(1) tăng cực độ hiệu năng
            var childrenMap = allList
                .Where(q => q.ParentId.HasValue)
                .GroupBy(q => q.ParentId.Value)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            var roots = allList.Where(q => q.ParentId == null).ToList();
            var result = new List<Question>();

            foreach (var root in roots)
            {
                var current = root;

                while (current != null && !visited.Contains(current.Id))
                {
                    result.Add(current);
                    visited.Add(current.Id);

                    // Tra cứu trực tiếp từ Dictionary thay vì FirstOrDefault trên danh sách gốc
                    if (childrenMap.TryGetValue(current.Id, out var child))
                    {
                        current = child;
                    }
                    else
                    {
                        current = null;
                    }
                }
            }

            return result;
        }

        private void InsertAtPosition(Question newQ, int position, List<Question> ordered)
        {
            if (position < 1 || position > ordered.Count + 1)
                position = ordered.Count + 1; // append cuối

            if (position == 1) // chèn đầu
            {
                newQ.ParentId = null;
                if (ordered.Any())
                    ordered[0].ParentId = newQ.Id;
            }
            else
            {
                var prev = ordered[position - 2];           // vị trí trước
                var next = position <= ordered.Count ? ordered[position - 1] : null;

                newQ.ParentId = prev.Id;
                if (next != null)
                    next.ParentId = newQ.Id;
            }
        }

        private void MoveToNewPosition(Question question, int newPosition, List<Question> ordered)
        {
            // Trong UpdateAsync, dữ liệu lấy từ DB có thể khác instance nên phải so sánh bằng ID
            var currentIndex = ordered.FindIndex(q => q.Id == question.Id);
            
            if (currentIndex >= 0)
            {
                // Phải nối lại các node trước khi dời (A -> B -> C), gỡ B ra thì A phải trỏ nối vào C
                var prevNode = currentIndex > 0 ? ordered[currentIndex - 1] : null;
                var nextNode = currentIndex + 1 < ordered.Count ? ordered[currentIndex + 1] : null;

                if (nextNode != null)
                {
                    nextNode.ParentId = prevNode?.Id;
                }

                ordered.RemoveAt(currentIndex);
            }

            // Chèn Node vào chỗ mới
            InsertAtPosition(question, newPosition, ordered);
        }
    }
}
