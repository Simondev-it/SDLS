namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonDTO
    {
        public Guid Id { get; set; }
        public Guid QuestionChapterId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public int? Status { get; set; }
        public List<QuestionLessonImageDTO> LessonImages { get; set; } = new();
    }
}