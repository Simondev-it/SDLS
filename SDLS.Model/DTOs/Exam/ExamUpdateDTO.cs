using SDLS.Model.DTOs.Answer.ExamQuestion;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamUpdateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        [Range(1, 600, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Duration { get; set; }

        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? PassScore { get; set; }

        public bool IsRandom { get; set; }

        public List<ExamQuestionUpdateDTO> ExamQuestions { get; set; } = new();
    }
}