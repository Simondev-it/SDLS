namespace SDLS.Model.DTOs.Resolve
{
    public class ResolveDTO
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}