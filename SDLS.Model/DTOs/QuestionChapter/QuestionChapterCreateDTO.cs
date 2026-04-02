using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.QuestionChapter
{
    public class QuestionChapterCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public Guid DrivingLicenseId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }
    }
}