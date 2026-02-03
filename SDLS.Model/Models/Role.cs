using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("role")]
[Index("Name", Name = "role_name_key", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<Rolepermission> Rolepermissions { get; set; } = new List<Rolepermission>();

    [InverseProperty("RoleNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
