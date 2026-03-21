using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("TrafficSign")]
[Index("Code", Name = "TrafficSign_code_key", IsUnique = true)]
[Index("Name", Name = "TrafficSign_name_key", IsUnique = true)]
public partial class TrafficSign
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("signCategoryId")]
    public Guid SignCategoryId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("code")]
    [StringLength(255)]
    public string Code { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("vectorData")]
    [StringLength(255)]
    public string? VectorData { get; set; }

    [Column("image")]
    [StringLength(255)]
    public string? Image { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("TrafficSign")]
    public virtual ICollection<SavedTrafficSign> SavedTrafficSigns { get; set; } = new List<SavedTrafficSign>();

    [ForeignKey("SignCategoryId")]
    [InverseProperty("TrafficSigns")]
    public virtual SignCategory SignCategory { get; set; } = null!;
}
