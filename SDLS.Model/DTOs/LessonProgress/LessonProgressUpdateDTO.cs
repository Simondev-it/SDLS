using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.LessonProgress
{
    public class LessonProgressUpdateDTO
    {
        [NotEmptyGuid]
        public Guid QuestionLessonId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Score { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}