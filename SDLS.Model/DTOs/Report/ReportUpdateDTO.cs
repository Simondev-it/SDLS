using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Report
{
    public class ReportUpdateDTO
    {
        public Guid? SimulationId { get; set; }
        public Guid? ForumPostId { get; set; }
        public Guid? ForumCommentId { get; set; }
        public Guid? QuestionId { get; set; }

        [NotEmptyGuid]
        public Guid ReportCategoryId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Vượt quá độ dài tối đa 255 ký tự.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Vượt quá độ dài tối đa 1000 ký tự.")]
        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        //[Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}