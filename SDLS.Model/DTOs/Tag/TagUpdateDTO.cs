using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Tag
{
    public class TagUpdateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Name { get; set; } = null!;

        
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string ColorCode { get; set; } = null!;

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}