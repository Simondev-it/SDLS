using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("jwks", Schema = "neon_auth")]
public partial class Jwk
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("publicKey")]
    public string PublicKey { get; set; } = null!;

    [Column("privateKey")]
    public string PrivateKey { get; set; } = null!;

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("expiresAt")]
    public DateTime? ExpiresAt { get; set; }
}
