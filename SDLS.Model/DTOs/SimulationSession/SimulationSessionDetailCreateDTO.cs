using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionDetailCreateDTO
    {
        [NotEmptyGuid]
        public Guid SimulationExamId { get; set; }

        [Range(0d, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public double? DurationSecond { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Score { get; set; }
    }
}