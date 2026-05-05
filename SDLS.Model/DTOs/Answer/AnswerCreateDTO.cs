using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Answer
{
    public class AnswerCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public string Content { get; set; } = null!;

        public bool IsCorrect { get; set; }
    }
}
