using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionDetailUpdateDTO
    {
        public Guid? Id { get; set; }

        [NotEmptyGuid]
        public Guid SimulationExamId { get; set; }

        [Range(0d, double.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public double? DurationSecond { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Score { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}