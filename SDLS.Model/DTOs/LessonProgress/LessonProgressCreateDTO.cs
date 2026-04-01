using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.LessonProgress
{
    public class LessonProgressCreateDTO
    {
        [NotEmptyGuid]
        public Guid QuestionLessonId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá tr? không h?p l?.")]
        public int? Score { get; set; }
    }
}