using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class RandomExamQuestionChapterRatioDTO
    {
        [NotEmptyGuid]
        public Guid ChapterId { get; set; }

        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [Range(0, 100, ErrorMessage = "Ph?n tr?m ph?i t? 0 ??n 100.")]
        public int? Percentage { get; set; }
    }
}
