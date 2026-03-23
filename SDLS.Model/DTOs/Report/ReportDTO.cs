using SDLS.Model.DTOs.ForumComment;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Model.DTOs.Question;
using SDLS.Model.DTOs.ReportCategory;
using SDLS.Model.DTOs.SimulationScenario;

namespace SDLS.Model.DTOs.Report
{
    public class ReportDTO
    {
        public Guid Id { get; set; }
        public Guid? SimulationId { get; set; }
        public Guid? ForumPostId { get; set; }
        public Guid? ForumCommentId { get; set; }
        public Guid? QuestionId { get; set; }
        public Guid ReportCategoryId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Image { get; set; }
        public int? Status { get; set; }

        public ReportCategoryDTO? ReportCategory { get; set; }
        public ForumCommentDTO? ForumComment { get; set; }
        public ForumPostDTO? ForumPost { get; set; }
        public QuestionDTO? Question { get; set; }
        public SimulationScenarioDTO? Simulation { get; set; }
    }
}