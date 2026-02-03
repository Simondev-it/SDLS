using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("verification", Schema = "neon_auth")]
[Index("Identifier", Name = "verification_identifier_idx")]
public partial class Verification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("identifier")]
    public string Identifier { get; set; } = null!;

    [Column("value")]
    public string Value { get; set; } = null!;

    [Column("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
