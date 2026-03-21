using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Tag")]
[Index("ColorCode", Name = "Tag_colorCode_key", IsUnique = true)]
[Index("Name", Name = "Tag_name_key", IsUnique = true)]
public partial class Tag
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("colorCode")]
    [StringLength(255)]
    public string ColorCode { get; set; } = null!;

    [InverseProperty("Tag")]
    public virtual ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
}
