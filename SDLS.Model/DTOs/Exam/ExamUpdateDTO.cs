using SDLS.Model.DTOs.Answer.ExamQuestion;
using System;
using System.Collections.Generic;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamUpdateDTO
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public int? PassScore { get; set; }
        public bool IsRandom { get; set; }

        public List<ExamQuestionUpdateDTO> ExamQuestions { get; set; } = new();
    }
}