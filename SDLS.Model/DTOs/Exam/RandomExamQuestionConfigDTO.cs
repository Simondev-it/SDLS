using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class RandomExamQuestionConfigDTO
    {
        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [Range(1, int.MaxValue, ErrorMessage = "T?ng s? câu h?i ph?i l?n h?n 0.")]
        public int? TotalQuestions { get; set; }

        [MinLength(1, ErrorMessage = "Ph?i có ít nh?t 1 chapter.")]
        public List<RandomExamQuestionChapterRatioDTO>? ChapterRatios { get; set; }
    }
}
