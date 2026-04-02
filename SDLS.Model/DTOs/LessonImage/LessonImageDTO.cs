namespace SDLS.Model.DTOs.LessonImage
{
    public class LessonImageDTO
    {
        public Guid Id { get; set; }
        public Guid QuestionLessonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? Status { get; set; }
    }
}
