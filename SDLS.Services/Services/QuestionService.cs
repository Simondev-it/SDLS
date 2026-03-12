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

        public QuestionService(IQuestionRepository repository)
        {
            _repository = repository;
        }

        private QuestionDTO MapToDTO(Question question)
        {
            return new QuestionDTO
            {
                Id = question.Id,
                Questiondifficultylevelid = question.Questiondifficultylevelid,
                Content = question.Content,
                Image = question.Image,
                Explanation = question.Explanation,
                Issingleanswer = question.Issingleanswer,
                Createat = question.Createat,
                Updateat = question.Updateat,
                Status = question.Status,
                Answers = question.Answers.Select(a => new AnswerDTO
                {
                    Id = a.Id,
                    Content = a.Content,
                    Iscorrect = a.Iscorrect
                }).ToList()
            };
        }

        private Question MapCreateDTOToEntity(QuestionCreateDTO dto)
        {
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Questioncategoryid = dto.Questioncategoryid,
                Questiondifficultylevelid = dto.Questiondifficultylevelid,
                Content = dto.Content,
                Image = dto.Image,
                Explanation = dto.Explanation,
                Issingleanswer = dto.Issingleanswer,
                Createat = DateTime.UtcNow,
                Updateat = DateTime.UtcNow,
                Status = dto.Status
            };

            foreach (var answerDto in dto.Answers)
            {
                question.Answers.Add(new Answer
                {
                    Id = Guid.NewGuid(),
                    Questionid = question.Id,
                    Content = answerDto.Content,
                    Iscorrect = answerDto.Iscorrect,
                    Createat = DateTime.UtcNow,
                    Updateat = DateTime.UtcNow,
                    Status = question.Status // Assuming same status
                });
            }

            return question;
        }

        public async Task<QuestionDTO> GetByIdAsync(Guid id)
        {
            var question = await _repository.GetByIdAsync(id);
            return question != null ? MapToDTO(question) : null;
        }

        public async Task<IEnumerable<QuestionDTO>> GetAllAsync()
        {
            var questions = await _repository.GetAllAsync();
            return questions.Select(MapToDTO);
        }

        public async Task<QuestionDTO> CreateAsync(QuestionCreateDTO dto)
        {
            var question = MapCreateDTOToEntity(dto);
            await _repository.AddAsync(question);
            return MapToDTO(question);
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
