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
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Content { get; set; } = null!;

        public bool Iscorrect { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}
