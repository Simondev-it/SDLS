namespace SDLS.Model.DTOs.SituationExam
{
    public class SituationExamDTO
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public int? PassScore { get; set; }
        public bool IsRandom { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public List<SimulationExamDTO> SimulationExams { get; set; } = new();
    }
}