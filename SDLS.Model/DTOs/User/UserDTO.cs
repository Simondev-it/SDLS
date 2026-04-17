using SDLS.Model.DTOs.ExamSession;
using SDLS.Model.DTOs.ExamSession;
using SDLS.Model.DTOs.LearningProgress;
using SDLS.Model.DTOs.LessonProgress;
using SDLS.Model.DTOs.Role;
using SDLS.Model.DTOs.SimulationSession;
using SDLS.Model.DTOs.UserLicense;

namespace SDLS.Model.DTOs.User
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Description { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? LicenseType { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? Status { get; set; }

        public int LearningProgressQuestionCount { get; set; }
        public int TotalQuestionCount { get; set; }
        public int TotalQuestionLessonCount { get; set; }
        public double ExamPassRate { get; set; }
        public double SimulationPassRate { get; set; }

        public RoleDTO? Role { get; set; }
        public List<LearningProgressDTO> LearningProgresses { get; set; } = new();
        public List<ExamSessionDTO> ExamSessions { get; set; } = new();
        public List<UserLicenseDTO> UserLicenses { get; set; } = new();
        public List<LessonProgressDTO> LessonProgresses { get; set; } = new();
        public List<SimulationSessionDTO> SimulationSessions { get; set; } = new();
    }
}
