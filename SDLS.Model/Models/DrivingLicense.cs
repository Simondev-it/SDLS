using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("DrivingLicense")]
//[Index("Name", Name = "DrivingLicense_name_key", IsUnique = true)]
public partial class DrivingLicense
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

    [InverseProperty("DrivingLicense")]
    public virtual ICollection<QuestionChapter> QuestionChapters { get; set; } = new List<QuestionChapter>();

    [InverseProperty("DrivingLicense")]
    public virtual ICollection<UserLicense> UserLicenses { get; set; } = new List<UserLicense>();

    [InverseProperty("DrivingLicense")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
