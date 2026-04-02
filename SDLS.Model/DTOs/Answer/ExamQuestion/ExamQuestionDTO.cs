using SDLS.Model.DTOs.Question;
using System;

namespace SDLS.Model.DTOs.Answer.ExamQuestion
{
    public class ExamQuestionDTO
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public int? Status { get; set; }

        // 1 ExamQuestion -> 1 Question
        public QuestionDTO? Question { get; set; }
    }
}
