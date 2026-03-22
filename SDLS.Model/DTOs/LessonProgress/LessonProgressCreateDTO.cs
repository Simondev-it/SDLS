using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.LessonProgress
{
    public class LessonProgressCreateDTO
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [NotEmptyGuid]
        public Guid QuestionLessonId { get; set; }
    }
}