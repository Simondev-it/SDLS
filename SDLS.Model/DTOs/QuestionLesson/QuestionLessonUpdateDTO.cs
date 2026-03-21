namespace SDLS.Model.DTOs.QuestionLesson
{
    public class QuestionLessonUpdateDTO
    {
        public Guid QuestionChapterId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }
        public List<QuestionLessonImageUpdateDTO> LessonImages { get; set; } = new();
    }
}