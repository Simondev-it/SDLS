using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Exam
{
    public class RandomExamQuestionConfigDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tổng số câu hỏi phải lớn hơn 0.")]
        public int? TotalQuestions { get; set; }

        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 chapter.")]
        public List<RandomExamQuestionChapterRatioDTO>? ChapterRatios { get; set; }
    }
}
