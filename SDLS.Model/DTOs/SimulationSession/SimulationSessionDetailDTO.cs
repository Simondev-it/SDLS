namespace SDLS.Model.DTOs.SimulationSession
{
    public class SimulationSessionDetailDTO
    {
        public Guid Id { get; set; }
        public Guid SimulationExamId { get; set; }
        public double? DurationSecond { get; set; }
        public int? Score { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}