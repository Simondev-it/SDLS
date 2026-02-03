using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("session", Schema = "neon_auth")]
[Index("Token", Name = "session_token_key", IsUnique = true)]
[Index("UserId", Name = "session_userId_idx")]
public partial class Session
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [Column("token")]
    public string Token { get; set; } = null!;

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [Column("ipAddress")]
    public string? IpAddress { get; set; }

    [Column("userAgent")]
    public string? UserAgent { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("impersonatedBy")]
    public string? ImpersonatedBy { get; set; }

    [Column("activeOrganizationId")]
    public string? ActiveOrganizationId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Sessions")]
    public virtual User1 User { get; set; } = null!;
}
