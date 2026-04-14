using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SystemConfig
{
    public class SystemConfigUpdateDTO
    {
        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [StringLength(255, ErrorMessage = "V??t quá ?? dài t?i ?a 255 ký t?.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        public int Value { get; set; }

        [StringLength(1000, ErrorMessage = "V??t quá ?? dài t?i ?a 1000 ký t?.")]
        public string? Description { get; set; }

        [Range(0, 1, ErrorMessage = "Giá tr? không h?p l?.")]
        public int? Status { get; set; }
    }
}
