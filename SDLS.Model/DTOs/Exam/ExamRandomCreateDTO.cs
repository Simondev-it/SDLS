using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class ExamRandomCreateDTO
    {
        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [StringLength(255, ErrorMessage = "V??t quá ?? dài t?i ?a 255 ký t?.")]
        public string Title { get; set; } = null!;

        [StringLength(255, ErrorMessage = "V??t quá ?? dài t?i ?a 255 ký t?.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [Range(0d, double.MaxValue, ErrorMessage = "Giá tr? không h?p l?.")]
        public double? Duration { get; set; }

        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [Range(0, 100, ErrorMessage = "Giá tr? không h?p l?.")]
        public int? PassScore { get; set; }

        public RandomExamQuestionConfigDTO? RandomQuestionConfig { get; set; }
    }
}
