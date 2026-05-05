using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.TrafficSign
{
    public class TrafficSignCreateDTO
    {
        [NotEmptyGuid]
        public Guid SignCategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Index { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Code { get; set; } = null!;

        
        public string? Description { get; set; }

        
        public string? VectorData { get; set; }

        public string? Image { get; set; }
    }
}