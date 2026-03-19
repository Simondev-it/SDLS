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
            int pageSize = 20)
        {
            //if (!lessonId.HasValue)
            //    throw new ArgumentException("LessonId is required");

            //var allQuestions = await _questionRepository.GetAllByLessonAsync(lessonId.Value);
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
            if (!dto.Answers.Any() || !dto.Answers.Any(a => a.Iscorrect))
                throw new ArgumentException("Phải có ít nhất 1 đáp án đúng");

            var all = await _questionRepository.GetAllByLessonAsync(dto.QuestionLessonId);
            var ordered = BuildOrderedLinkedList(all);

            var newQuestion = _mapper.Map<Question>(dto);
            newQuestion.Id = Guid.NewGuid();
            newQuestion.CreateAt = DateTime.UtcNow.ToLocalTime();
            newQuestion.UpdateAt = DateTime.UtcNow.ToLocalTime();
            newQuestion.Status = 1;

            // Map answers
            foreach (var ans in newQuestion.Answers)
            {
                ans.Id = Guid.NewGuid();
                ans.QuestionId = newQuestion.Id;
                ans.CreateAt = DateTime.UtcNow.ToLocalTime();
                ans.Status = 1;
            }

            // === CHÈN VÀO VỊ TRÍ ===
            InsertAtPosition(newQuestion, dto.Position, ordered);

            await _questionRepository.AddAsync(newQuestion);

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionCreateDTO dto)
        {
            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Question not found");

            _mapper.Map(dto, existing);
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            // Xử lý Logic LinkedList (Tránh vòng lặp vô tận: A -> B -> A)
            if (existing.ParentId == existing.Id)
                throw new ArgumentException("A question cannot be its own parent.");

            // 4. Cập nhật Answers (Xóa cũ thêm mới)
            // Để EF hiểu việc xóa, bạn cần đảm bảo Answers là một Tracking Collection
            existing.Answers.Clear();

            foreach (var answerDto in dto.Answers)
            {
                var newAnswer = _mapper.Map<Answer>(answerDto);
                newAnswer.Id = Guid.NewGuid();
                newAnswer.QuestionId = existing.Id;
                newAnswer.CreateAt = DateTime.UtcNow.ToLocalTime();
                newAnswer.Status = 1;
                existing.Answers.Add(newAnswer);
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
            var dict = all.ToDictionary(q => q.Id);
            var roots = all.Where(q => q.ParentId == null).ToList();

            var result = new List<Question>();

            foreach (var root in roots)   // thường chỉ có 1 root
            {
                var current = root;
                while (current != null)
                {
                    result.Add(current);
                    // Tìm next (chỉ có 1 next vì singly linked list)
                    current = all.FirstOrDefault(q => q.ParentId == current.Id);
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
            // Xóa tạm khỏi list (để tránh lặp)
            ordered.Remove(question);
            InsertAtPosition(question, newPosition, ordered);
        }
    }
}
