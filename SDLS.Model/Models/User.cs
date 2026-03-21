using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("User")]
[Index("Email", Name = "User_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("roleId")]
    public Guid RoleId { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("avatar")]
    [StringLength(255)]
    public string? Avatar { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("gender")]
    [StringLength(20)]
    public string? Gender { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("licenseType")]
    [StringLength(20)]
    public string? LicenseType { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<CommentVote> CommentVotes { get; set; } = new List<CommentVote>();

    [InverseProperty("User")]
    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();

    [InverseProperty("User")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("User")]
    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    [InverseProperty("User")]
    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    [InverseProperty("User")]
    public virtual LearningProgress? LearningProgress { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    [InverseProperty("User")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("User")]
    public virtual ICollection<PostReact> PostReacts { get; set; } = new List<PostReact>();

    [InverseProperty("User")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [InverseProperty("User")]
    public virtual ICollection<Resolve> Resolves { get; set; } = new List<Resolve>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<SavedQuestion> SavedQuestions { get; set; } = new List<SavedQuestion>();

    [InverseProperty("User")]
    public virtual ICollection<SavedTrafficSign> SavedTrafficSigns { get; set; } = new List<SavedTrafficSign>();

    [InverseProperty("User")]
    public virtual ICollection<SimulationSession> SimulationSessions { get; set; } = new List<SimulationSession>();

    [InverseProperty("User")]
    public virtual ICollection<UserLicense> UserLicenses { get; set; } = new List<UserLicense>();

    [InverseProperty("User")]
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
