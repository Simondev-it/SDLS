using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SituationExam
{
    public class SimulationExamCreateDTO
    {
        [NotEmptyGuid]
        public Guid SimulationId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? BaseScore { get; set; }
    }
}