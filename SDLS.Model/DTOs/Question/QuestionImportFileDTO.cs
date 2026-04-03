using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Question
{
    public class QuestionImportFileDTO
    {
        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        public IFormFile File { get; set; } = null!;
    }
}
