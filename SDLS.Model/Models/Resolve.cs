using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class Resolve
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual Report Report { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
