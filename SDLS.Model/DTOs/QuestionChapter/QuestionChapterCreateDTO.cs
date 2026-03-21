namespace SDLS.Model.DTOs.QuestionChapter
{
    public class QuestionChapterCreateDTO
    {
        public Guid DrivingLicenseId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}