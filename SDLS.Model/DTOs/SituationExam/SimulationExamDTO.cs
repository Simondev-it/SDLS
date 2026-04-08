using SDLS.Model.DTOs.SimulationScenario;

namespace SDLS.Model.DTOs.SituationExam
{
    public class SimulationExamDTO
    {
        public Guid Id { get; set; }
        public Guid SituationExamId { get; set; }
        public Guid SimulationId { get; set; }
        public int? BaseScore { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public SimulationScenarioDTO? Simulation { get; set; }
    }
}