using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionUpdateDTO
    {
        [NotEmptyGuid]
        public Guid SituationExamId { get; set; }

        public List<SimulationSessionDetailUpdateDTO> SimulationSessionDetails { get; set; } = new();

        public int? Status { get; set; }
    }
}