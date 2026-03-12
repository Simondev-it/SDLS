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
        private readonly IQuestionRepository _repository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<QuestionDTO>> GetAllAsync()
        {
            var questions = await _repository.GetAllAsync();
            return _mapper.Map<List<QuestionDTO>>(questions);
        }

        public async Task<QuestionDTO> GetByIdAsync(Guid id)
        {
            var question = await _repository.GetByIdAsync(id);
            if (question == null)
                throw new KeyNotFoundException($"Không tìm thấy câu hỏi với ID {id}");

            return _mapper.Map<QuestionDTO>(question);
        }


        public async Task<QuestionDTO> CreateAsync(QuestionCreateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (!dto.Answers.Any(a => a.Iscorrect))
                throw new InvalidOperationException("Phải có ít nhất một đáp án đúng");

            var question = _mapper.Map<Question>(dto);

            question.Id = Guid.NewGuid();
            question.CreateAt = DateTime.UtcNow;
            question.UpdateAt = DateTime.UtcNow;
            question.Status = 1; // active

            // Gán QuestionId cho các answer
            foreach (var answer in question.Answers)
            {
                answer.Id = Guid.NewGuid();
                answer.QuestionId = question.Id;
                answer.CreateAt = DateTime.UtcNow;
            }

            await _questionRepo.CreateWithAnswersAsync(question);

            var created = await _questionRepo.GetByIdWithAnswersAsync(question.Id);
            return _mapper.Map<QuestionDTO>(created);
        }

        public async Task<QuestionDTO> UpdateAsync(Guid id, QuestionUpdateDTO dto)
        {
            var existingQuestion = await _repository.GetByIdAsync(id);
            if (existingQuestion == null) return null;

            existingQuestion.Questioncategoryid = dto.Questioncategoryid ?? existingQuestion.Questioncategoryid;
            existingQuestion.Questiondifficultylevelid = dto.Questiondifficultylevelid ?? existingQuestion.Questiondifficultylevelid;
            existingQuestion.Content = dto.Content;
            existingQuestion.Image = dto.Image ?? existingQuestion.Image;
            existingQuestion.Explanation = dto.Explanation ?? existingQuestion.Explanation;
            existingQuestion.Issingleanswer = dto.Issingleanswer ?? existingQuestion.Issingleanswer;
            existingQuestion.Updateat = DateTime.UtcNow;
            existingQuestion.Status = dto.Status ?? existingQuestion.Status;

            // Handle Answers: Update existing, add new, remove deleted (assuming all provided)
            var existingAnswerIds = existingQuestion.Answers.Select(a => a.Id).ToHashSet();
            var updatedAnswerIds = new HashSet<Guid>();

            foreach (var answerDto in dto.Answers)
            {
                if (answerDto.Id.HasValue && existingAnswerIds.Contains(answerDto.Id.Value))
                {
                    var existingAnswer = existingQuestion.Answers.First(a => a.Id == answerDto.Id.Value);
                    existingAnswer.Content = answerDto.Content;
                    existingAnswer.Iscorrect = answerDto.Iscorrect;
                    existingAnswer.Updateat = DateTime.UtcNow;
                    updatedAnswerIds.Add(answerDto.Id.Value);
                }
                else
                {
                    // New answer
                    var newAnswer = new Answer
                    {
                        Id = Guid.NewGuid(),
                        Questionid = existingQuestion.Id,
                        Content = answerDto.Content,
                        Iscorrect = answerDto.Iscorrect,
                        Createat = DateTime.UtcNow,
                        Updateat = DateTime.UtcNow,
                        Status = existingQuestion.Status
                    };
                    existingQuestion.Answers.Add(newAnswer);
                }
            }

            // Remove answers not in updated list
            var answersToRemove = existingQuestion.Answers.Where(a => a.Id != Guid.Empty && !updatedAnswerIds.Contains(a.Id)).ToList();
            foreach (var answer in answersToRemove)
            {
                existingQuestion.Answers.Remove(answer);
            }

            await _repository.UpdateAsync(existingQuestion);
            return MapToDTO(existingQuestion);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
