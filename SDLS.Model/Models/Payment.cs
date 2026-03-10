using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Method { get; set; }

    public int? Amount { get; set; }

    public string? Note { get; set; }

    public string? Response { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
