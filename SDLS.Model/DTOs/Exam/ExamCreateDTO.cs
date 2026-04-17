using SDLS.Model.DTOs.Answer.ExamQuestion;
using SDLS.Model.DTOs.Answer.ExamQuestion;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(0d, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public double? Duration { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? PassScore { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public bool IsRandom { get; set; }

        [MinLength(1, ErrorMessage = "Trường này là bắt buộc.")]
        public List<ExamQuestionCreateDTO> ExamQuestions { get; set; } = new();
    }
}
