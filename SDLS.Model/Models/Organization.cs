using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("organization", Schema = "neon_auth")]
[Index("Slug", Name = "organization_slug_key", IsUnique = true)]
[Index("Slug", Name = "organization_slug_uidx", IsUnique = true)]
public partial class Organization
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("slug")]
    public string Slug { get; set; } = null!;

    [Column("logo")]
    public string? Logo { get; set; }

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    [Column("metadata")]
    public string? Metadata { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    [InverseProperty("Organization")]
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
