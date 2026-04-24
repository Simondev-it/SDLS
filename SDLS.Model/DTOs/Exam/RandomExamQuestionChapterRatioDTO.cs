using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class RandomExamQuestionChapterRatioDTO
    {
        [NotEmptyGuid]
        public Guid ChapterId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "Phần trăm phải từ 0 đến 100.")]
        public int? Percentage { get; set; }
    }
}
