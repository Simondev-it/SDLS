using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonUpdateDTO
    {
        public Guid? QuestionChapterId { get; set; }

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Name { get; set; }

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        public string? Content { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}