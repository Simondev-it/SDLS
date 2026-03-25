using SDLS.Model.DTOs.SimulationSession;
using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionCreateDTO
    {
        [NotEmptyGuid]
        public Guid SituationExamId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [MinLength(1, ErrorMessage = "Trường này là bắt buộc.")]
        public List<SimulationSessionDetailCreateDTO> SimulationSessionDetails { get; set; } = new();
    }
}