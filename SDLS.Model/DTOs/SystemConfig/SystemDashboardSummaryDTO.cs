namespace SDLS.Model.DTOs.SystemConfig
{
    public class SystemDashboardSummaryDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long TotalPaymentAmount { get; set; }

        public int MonthlyExamSessionCount { get; set; }
        public int MonthlySimulationSessionCount { get; set; }

        public SessionStatsDTO WeeklyExamStats { get; set; } = new();
        public SessionStatsDTO WeeklySimulationStats { get; set; } = new();
        public SessionStatsDTO MonthlyExamStats { get; set; } = new();
        public SessionStatsDTO MonthlySimulationStats { get; set; } = new();

        public List<RoleUserStatsDTO> UserRoleStats { get; set; } = new();

        public int PendingReviewReportsCount { get; set; }
        public int PendingReviewReportsIncreaseFromYesterday { get; set; }

        public int PendingForumPostsCount { get; set; }

        public int PendingContentChangeReportsCount { get; set; }
        public int PendingContentChangeReportsIncreaseFromYesterday { get; set; }
    }

    public class SessionStatsDTO
    {
        public int TotalCount { get; set; }
        public double PassRate { get; set; }
        public double FailRate { get; set; }
    }

    public class RoleUserStatsDTO
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public double ActiveRate { get; set; }
    }
}
