using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class DrivingLicense
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public Guid? Binding { get; set; }

    public virtual ICollection<QuestionChapter> QuestionChapters { get; set; } = new List<QuestionChapter>();

    public virtual ICollection<UserLicense> UserLicenses { get; set; } = new List<UserLicense>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
