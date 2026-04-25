namespace SDLS.Model.DTOs.Report
{
    public class ReportUserBriefDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string RoleName { get; set; }
        public int? Status { get; set; }
    }
}
