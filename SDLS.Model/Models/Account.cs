using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("account", Schema = "neon_auth")]
[Index("UserId", Name = "account_userId_idx")]
public partial class Account
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("accountId")]
    public string AccountId { get; set; } = null!;

    [Column("providerId")]
    public string ProviderId { get; set; } = null!;

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("accessToken")]
    public string? AccessToken { get; set; }

    [Column("refreshToken")]
    public string? RefreshToken { get; set; }

    [Column("idToken")]
    public string? IdToken { get; set; }

    [Column("accessTokenExpiresAt")]
    public DateTime? AccessTokenExpiresAt { get; set; }

    [Column("refreshTokenExpiresAt")]
    public DateTime? RefreshTokenExpiresAt { get; set; }

    [Column("scope")]
    public string? Scope { get; set; }

    [Column("password")]
    public string? Password { get; set; }

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Accounts")]
    public virtual User1 User { get; set; } = null!;
}
