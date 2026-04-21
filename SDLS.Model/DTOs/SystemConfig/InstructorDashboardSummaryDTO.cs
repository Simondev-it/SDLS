using SDLS.Model.DTOs.ForumPost;
using SDLS.Model.DTOs.Report;

namespace SDLS.Model.DTOs.SystemConfig
{
    public class InstructorDashboardSummaryDTO
    {
        public int ForumPostCount { get; set; }
        public List<ForumPostDTO> ForumPosts { get; set; } = new();

        public int ContentChangeRequestCount { get; set; }
        public List<ReportDTO> ContentChangeRequests { get; set; } = new();

        public int ContentIssueReportCount { get; set; }
        public int CommunityReportCount { get; set; }
    }
}
