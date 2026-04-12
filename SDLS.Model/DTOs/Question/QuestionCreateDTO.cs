using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.QuestionTag;
using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Question
{
    public class QuestionCreateDTO
    {
        [NotEmptyGuid]
        public Guid QuestionLessonId { get; set; }

        [NotEmptyGuid]
        public Guid QuestionTopicId { get; set; }

        [NotEmptyGuid]
        public Guid QuestionCategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Index { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Explanation { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Vượt quá độ dài tối đa 20 ký tự.")]
        public string? Type { get; set; }

        [MinLength(1, ErrorMessage = "Trường này là bắt buộc.")]
        public List<AnswerCreateDTO> Answers { get; set; } = new();

        public List<QuestionTagCreateDTO> QuestionTags { get; set; } = new();
    }
}
