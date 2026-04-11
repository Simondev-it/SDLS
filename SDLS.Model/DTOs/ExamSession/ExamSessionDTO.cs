using SDLS.Model.DTOs.Exam;
using SDLS.Model.DTOs.ExamDetail;

namespace SDLS.Model.DTOs.ExamSession
{
    public class ExamSessionDTO
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid UserId { get; set; }
        public int? Score { get; set; }
        public double? TotalDuration { get; set; }
        public bool IsPassed { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public ExamDTO? Exam { get; set; }
        public List<ExamDetailDTO> ExamDetails { get; set; } = new();
    }
}