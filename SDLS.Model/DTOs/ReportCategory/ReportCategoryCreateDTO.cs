using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ReportCategory
{
    public class ReportCategoryCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Name { get; set; } = null!;

        
        public string? Description { get; set; }
    }
}