using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class UserLicense
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DrivingLicenseId { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual DrivingLicense DrivingLicense { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
