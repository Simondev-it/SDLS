using SDLS.Model.DTOs.DrivingLicense;

namespace SDLS.Model.DTOs.QuestionChapter
{
    public class QuestionChapterDTO
    {
        public Guid Id { get; set; }
        public Guid DrivingLicenseId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status { get; set; }

        public DrivingLicenseDTO? DrivingLicense { get; set; }
    }
}