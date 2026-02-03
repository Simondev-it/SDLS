using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("invitation", Schema = "neon_auth")]
[Index("Email", Name = "invitation_email_idx")]
[Index("OrganizationId", Name = "invitation_organizationId_idx")]
public partial class Invitation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("organizationId")]
    public Guid OrganizationId { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("role")]
    public string? Role { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    [Column("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("inviterId")]
    public Guid InviterId { get; set; }

    [ForeignKey("InviterId")]
    [InverseProperty("Invitations")]
    public virtual User1 Inviter { get; set; } = null!;

    [ForeignKey("OrganizationId")]
    [InverseProperty("Invitations")]
    public virtual Organization Organization { get; set; } = null!;
}
