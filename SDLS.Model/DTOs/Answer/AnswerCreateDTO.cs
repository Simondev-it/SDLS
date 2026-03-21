using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Answer
{
    public class AnswerCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Content { get; set; } = null!;

        public bool Iscorrect { get; set; }
    }
}
