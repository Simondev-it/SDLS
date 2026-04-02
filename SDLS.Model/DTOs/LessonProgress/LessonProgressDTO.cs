using SDLS.Model.DTOs.QuestionLesson;

namespace SDLS.Model.DTOs.LessonProgress
{
    public class LessonProgressDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionLessonId { get; set; }
        public int? Score { get; set; }
        public int? Status { get; set; }

        public QuestionLessonDTO? QuestionLesson { get; set; }
    }
}