using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Answer
{
    public class AnswerUpdateDTO
    {
        public Guid? Id { get; set; }

        [NotEmptyGuid]
        public Guid QuestionId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Content { get; set; } = null!;

        public bool IsCorrect { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}
