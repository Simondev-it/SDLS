using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("user", Schema = "neon_auth")]
[Index("Email", Name = "user_email_key", IsUnique = true)]
public partial class User1
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("emailVerified")]
    public bool EmailVerified { get; set; }

    [Column("image")]
    public string? Image { get; set; }

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [Column("role")]
    public string? Role { get; set; }

    [Column("banned")]
    public bool? Banned { get; set; }

    [Column("banReason")]
    public string? BanReason { get; set; }

    [Column("banExpires")]
    public DateTime? BanExpires { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    [InverseProperty("Inviter")]
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    [InverseProperty("User")]
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    [InverseProperty("User")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
