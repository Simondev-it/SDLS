using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("rolepermission")]
[Index("Roleid", "Permissionid", Name = "rolepermission_roleid_permissionid_key", IsUnique = true)]
public partial class Rolepermission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("roleid")]
    public Guid? Roleid { get; set; }

    [Column("permissionid")]
    public Guid? Permissionid { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("Permissionid")]
    [InverseProperty("Rolepermissions")]
    public virtual Permission? Permission { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("Rolepermissions")]
    public virtual Role? Role { get; set; }
}
