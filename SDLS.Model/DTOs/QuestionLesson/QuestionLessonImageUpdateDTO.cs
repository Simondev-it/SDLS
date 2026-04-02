namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonImageUpdateDTO
    {
        public Guid? Id { get; set; }
        public Guid QuestionLessonId { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int? Status { get; set; }
    }
}