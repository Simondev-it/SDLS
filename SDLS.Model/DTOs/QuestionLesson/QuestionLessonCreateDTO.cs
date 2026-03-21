namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonCreateDTO
    {
        public Guid QuestionChapterId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<QuestionLessonImageCreateDTO> LessonImages { get; set; } = new();
    }
}