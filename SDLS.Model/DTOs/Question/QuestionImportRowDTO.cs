namespace SDLS.Model.DTOs.Question
{
    public class QuestionImportRowDTO
    {
        public string QuestionLessonName { get; set; } = null!;
        public string QuestionTopicName { get; set; } = null!;
        public string QuestionCategoryName { get; set; } = null!;
        public int? Index { get; set; }
        public string Content { get; set; } = null!;
        public string? Image { get; set; }
        public string? Explanation { get; set; }
        public string? Type { get; set; }
        public string Answers { get; set; } = null!;
        public string? QuestionTagNames { get; set; }
    }
}
