using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamRandomCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Viết quá dài tối đa 255 ký tự.")]
        public string Title { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Viết quá dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(0d, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public double? Duration { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? PassScore { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "Phần trăm câu điểm liệt từ 0 đến 100.")]
        public int CriticalPercentage { get; set; } // Số câu điểm liệt (%)

        public int? Status { get; set; } = 1;

        public RandomExamQuestionConfigDTO? RandomQuestionConfig { get; set; }
    }
}
