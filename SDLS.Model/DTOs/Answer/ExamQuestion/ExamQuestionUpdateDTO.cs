using System;

namespace SDLS.Model.DTOs.Answer.ExamQuestion
{
    public class ExamQuestionUpdateDTO
    {
        public Guid? Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public int? Status { get; set; }
    }
}