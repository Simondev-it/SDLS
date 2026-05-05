using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.QuestionChapter
{
    public class QuestionChapterUpdateDTO
    {
        [NotEmptyGuid]
        public Guid DrivingLicenseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Index { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Name { get; set; } = null!;

        
        public string? Description { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}