using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("Vehicle")]
//[Index("Name", Name = "Vehicle_name_key", IsUnique = true)]
public partial class Vehicle
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("drivingLicenseId")]
    public Guid DrivingLicenseId { get; set; }

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

    [ForeignKey("DrivingLicenseId")]
    [InverseProperty("Vehicles")]
    public virtual DrivingLicense DrivingLicense { get; set; } = null!;
}
