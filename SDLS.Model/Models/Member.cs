using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("member", Schema = "neon_auth")]
[Index("OrganizationId", Name = "member_organizationId_idx")]
[Index("UserId", Name = "member_userId_idx")]
public partial class Member
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("organizationId")]
    public Guid OrganizationId { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("role")]
    public string Role { get; set; } = null!;

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Members")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Members")]
    public virtual User1 User { get; set; } = null!;
}
