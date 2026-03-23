namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionDetailDTO
    {
        public Guid Id { get; set; }
        public Guid SimulationExamId { get; set; }
        public int? DurationSecond { get; set; }
        public int? Score { get; set; }
        public int? Status { get; set; }
    }
}