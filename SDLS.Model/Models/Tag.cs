using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class Tag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public string ColorCode { get; set; } = null!;

    public virtual ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
}
