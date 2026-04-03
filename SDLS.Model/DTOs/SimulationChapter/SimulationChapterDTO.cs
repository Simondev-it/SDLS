namespace SDLS.Model.DTOs.SimulationChapter
{
    public class SimulationChapterDTO
    {
        public Guid Id { get; set; }
        public int? Index { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
    }
}