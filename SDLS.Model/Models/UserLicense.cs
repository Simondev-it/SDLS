using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("UserLicense")]
[Index("UserId", "DrivingLicenseId", Name = "UserLicense_userId_drivingLicenseId_key", IsUnique = true)]
public partial class UserLicense
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("userId")]
    public Guid UserId { get; set; }

    [Column("drivingLicenseId")]
    public Guid DrivingLicenseId { get; set; }

    [Column("createAt", TypeName = "timestamp without time zone")]
    public DateTime? CreateAt { get; set; }

    [Column("updateAt", TypeName = "timestamp without time zone")]
    public DateTime? UpdateAt { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [ForeignKey("DrivingLicenseId")]
    [InverseProperty("UserLicenses")]
    public virtual DrivingLicense DrivingLicense { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserLicenses")]
    public virtual User User { get; set; } = null!;
}
