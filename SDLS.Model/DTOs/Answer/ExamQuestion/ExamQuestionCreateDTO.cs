using System;
using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.Answer.ExamQuestion
{
    public class ExamQuestionCreateDTO
    {
        [NotEmptyGuid]
        public Guid QuestionId { get; set; }
    }
}