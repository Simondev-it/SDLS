using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public Guid QuestionChapterId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Index { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        public string? Content { get; set; }
    }
}