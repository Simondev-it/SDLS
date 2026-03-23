using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionDetailCreateDTO
    {
        [NotEmptyGuid]
        public Guid SimulationExamId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? DurationSecond { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Score { get; set; }
    }
}