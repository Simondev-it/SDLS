namespace SDLS.Model.DTOs.SimulationDifficultyLevel
{
    public class SimulationDifficultyLevelDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
    }
}