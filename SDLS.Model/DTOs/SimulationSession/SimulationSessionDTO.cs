namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionDTO
    {
        public Guid Id { get; set; }
        public Guid SituationExamId { get; set; }
        public Guid UserId { get; set; }
        public int? TotalScore { get; set; }
        public int? TotalDuration { get; set; }
        public bool IsPassed { get; set; }
        public int? Status { get; set; }

        public List<SimulationSessionDetailDTO> SimulationSessionDetails { get; set; } = new();
    }
}