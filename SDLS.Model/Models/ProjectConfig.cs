using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("project_config", Schema = "neon_auth")]
[Index("EndpointId", Name = "project_config_endpoint_id_key", IsUnique = true)]
public partial class ProjectConfig
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("endpoint_id")]
    public string EndpointId { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("trusted_origins", TypeName = "jsonb")]
    public string TrustedOrigins { get; set; } = null!;

    [Column("social_providers", TypeName = "jsonb")]
    public string SocialProviders { get; set; } = null!;

    [Column("email_provider", TypeName = "jsonb")]
    public string? EmailProvider { get; set; }

    [Column("email_and_password", TypeName = "jsonb")]
    public string? EmailAndPassword { get; set; }

    [Column("allow_localhost")]
    public bool AllowLocalhost { get; set; }
}
