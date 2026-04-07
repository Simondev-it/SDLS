using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Answer.ExamQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public double? Duration { get; set; }

        public int? PassScore { get; set; }

        public bool IsRandom { get; set; }

        public int? Status { get; set; }

        public List<ExamQuestionDTO> ExamQuestions { get; set; } = new();
    }
}
