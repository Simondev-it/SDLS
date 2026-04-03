using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SimulationScenario
{
    public class SimulationScenarioUpdateDTO
    {
        [NotEmptyGuid]
        public Guid SimulationChapterId { get; set; }

        [NotEmptyGuid]
        public Guid SimulationCategoryId { get; set; }

        [NotEmptyGuid]
        public Guid SimulationDifficultyLevelId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Index { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string? Video { get; set; }

        [Range(0d, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public double TotalTime { get; set; }

        public double StartPoint { get; set; }
        public double EndPoint { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}