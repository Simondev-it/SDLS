using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SimulationCategory
{
    public class SimulationCategoryCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Name { get; set; } = null!;

        
        public string? Description { get; set; }
    }
}