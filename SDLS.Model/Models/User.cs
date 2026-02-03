using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("User")]
[Index("Email", Name = "User_email_key", IsUnique = true)]
[Index("Username", Name = "User_username_key", IsUnique = true)]
[Index("Email", Name = "idx_user_email")]
[Index("Roleid", Name = "idx_user_role_id")]
[Index("Username", Name = "idx_user_username")]
public partial class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("roleid")]
    public Guid? Roleid { get; set; }

    [Column("username")]
    [StringLength(100)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("name")]
    [StringLength(200)]
    public string? Name { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("avatar")]
    public string? Avatar { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("gender")]
    [StringLength(20)]
    public string? Gender { get; set; }

    [Column("role")]
    [StringLength(50)]
    public string? Role { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("point")]
    public int? Point { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("dateofbirth")]
    public DateOnly? Dateofbirth { get; set; }

    [Column("licensetype")]
    [StringLength(50)]
    public string? Licensetype { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Eventsession> Eventsessions { get; set; } = new List<Eventsession>();

    [InverseProperty("User")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("User")]
    public virtual ICollection<Examsession> Examsessions { get; set; } = new List<Examsession>();

    [InverseProperty("User")]
    public virtual ICollection<Forumcomment> Forumcomments { get; set; } = new List<Forumcomment>();

    [InverseProperty("User")]
    public virtual ICollection<Forumpost> Forumposts { get; set; } = new List<Forumpost>();

    [InverseProperty("User")]
    public virtual ICollection<Learningprogress> Learningprogresses { get; set; } = new List<Learningprogress>();

    [InverseProperty("User")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("User")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("Roleid")]
    [InverseProperty("Users")]
    public virtual Role? RoleNavigation { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Savedquestion> Savedquestions { get; set; } = new List<Savedquestion>();

    [InverseProperty("User")]
    public virtual ICollection<Simulationsession> Simulationsessions { get; set; } = new List<Simulationsession>();

    [InverseProperty("User")]
    public virtual ICollection<Usernotification> Usernotifications { get; set; } = new List<Usernotification>();
}
