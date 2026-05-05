using SDLS.Model.DTOs.Answer.ExamQuestion;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamUpdateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0d, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public double? Duration { get; set; }

        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? PassScore { get; set; }

        public bool IsRandom { get; set; }

        public List<ExamQuestionUpdateDTO> ExamQuestions { get; set; } = new();
    }
}