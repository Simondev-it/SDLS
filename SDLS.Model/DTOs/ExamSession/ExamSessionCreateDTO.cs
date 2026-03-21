using SDLS.Model.DTOs.ExamDetail;
using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ExamSession
{
    public class ExamSessionCreateDTO
    {
        [NotEmptyGuid]
        public Guid ExamId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Score { get; set; }

        public bool IsPassed { get; set; }

        [MinLength(1, ErrorMessage = "Trường này là bắt buộc.")]
        public List<ExamDetailCreateDTO> ExamDetails { get; set; } = new();
    }
}