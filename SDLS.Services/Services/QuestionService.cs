using AutoMapper;
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
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IAnswerRepository answerRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<QuestionDTO>> GetAllAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return _mapper.Map<List<QuestionDTO>>(questions);
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
            if (dto.Answers == null || !dto.Answers.Any())
                throw new ArgumentException("Question must have at least 1 answer");

            if (!dto.Answers.Any(a => a.Iscorrect))
                throw new ArgumentException("At least one answer must be correct");

            var question = _mapper.Map<Question>(dto);

            question.Id = Guid.NewGuid();
            question.CreateAt = DateTime.UtcNow;
            question.UpdateAt = DateTime.UtcNow;
            question.Status = 1;

            
            foreach (var answer in question.Answers)
            {
                answer.Id = Guid.NewGuid();
                answer.QuestionId = question.Id;
                answer.CreateAt = DateTime.UtcNow;
                answer.Status = 1;
            }

            await _questionRepository.AddAsync(question);
            

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, QuestionCreateDTO dto)
        {
            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Question not found");

            // Validation
            if (!dto.Answers.Any(a => a.Iscorrect))
                throw new ArgumentException("At least one answer must be correct");

            // Map các field cơ bản (không map Answers)
            _mapper.Map(dto, existing);

            existing.UpdateAt = DateTime.UtcNow;

            // Xử lý Answers: cách đơn giản nhất là xóa hết cũ → thêm mới
            // (an toàn, dễ implement, phù hợp nếu không cần giữ history answer)

            existing.Answers.Clear(); // EF sẽ xóa các record cũ trong DB khi SaveChanges

            foreach (var answerDto in dto.Answers)
            {
                var newAnswer = _mapper.Map<Answer>(answerDto);
                newAnswer.Id = Guid.NewGuid();
                newAnswer.QuestionId = existing.Id;
                newAnswer.CreateAt = DateTime.UtcNow;
                newAnswer.Status = 1;

                existing.Answers.Add(newAnswer);
            }

            await _questionRepository.UpdateAsync(existing);
            // await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _questionRepository.DeleteAsync(id);
            return true;
        }
    }
}
