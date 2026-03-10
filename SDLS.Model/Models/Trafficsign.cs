using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class TrafficSign
{
    public Guid Id { get; set; }

    public Guid SignCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string? VectorData { get; set; }

    public string? Image { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<SavedTrafficSign> SavedTrafficSigns { get; set; } = new List<SavedTrafficSign>();

    public virtual SignCategory SignCategory { get; set; } = null!;
}
