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
        
        public string Name { get; set; } = null!;

        
        public string? Description { get; set; }

        public string? Content { get; set; }
    }
}