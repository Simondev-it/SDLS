using SDLS.Model.DTOs.Answer.ExamQuestion;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamCreateDTO
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public int? PassScore { get; set; }
        public bool IsRandom { get; set; }

        public List<ExamQuestionCreateDTO> ExamQuestions { get; set; } = new();
    }
}
