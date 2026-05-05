using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SignCategory
{
    public class SignCategoryCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Name { get; set; } = null!;

        
        public string? Description { get; set; }
    }
}