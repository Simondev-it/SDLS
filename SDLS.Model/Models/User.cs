using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class User
{
    public Guid Id { get; set; }

    public Guid RoleId { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

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

    public virtual ICollection<CommentVote> CommentVotes { get; set; } = new List<CommentVote>();

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();

    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<PostReact> PostReacts { get; set; } = new List<PostReact>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Resolve> Resolves { get; set; } = new List<Resolve>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SavedQuestion> SavedQuestions { get; set; } = new List<SavedQuestion>();

    public virtual ICollection<SavedTrafficSign> SavedTrafficSigns { get; set; } = new List<SavedTrafficSign>();

    public virtual ICollection<SimulationSession> SimulationSessions { get; set; } = new List<SimulationSession>();

    public virtual ICollection<UserLicense> UserLicenses { get; set; } = new List<UserLicense>();

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
