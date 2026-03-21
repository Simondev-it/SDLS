using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public Guid QuestionChapterId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        public string? Content { get; set; }

        // Upload multiple images from form-data
        public List<IFormFile> LessonImageFiles { get; set; } = new();

        // Optional custom names (same index with LessonImageFiles)
        public List<string>? LessonImageNames { get; set; }
    }
}