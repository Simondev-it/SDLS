using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class Report
{
    public Guid Id { get; set; }

    public Guid? SimulationId { get; set; }

    public Guid? ForumPostId { get; set; }

    public Guid? ForumCommentId { get; set; }

    public Guid? QuestionId { get; set; }

    public Guid ReportCategoryId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Image { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? Status { get; set; }

    public virtual ForumComment? ForumComment { get; set; }

    public virtual ForumPost? ForumPost { get; set; }

    public virtual Question? Question { get; set; }

    public virtual ReportCategory ReportCategory { get; set; } = null!;

    public virtual ICollection<Resolve> Resolves { get; set; } = new List<Resolve>();

    public virtual SimulationScenario? Simulation { get; set; }

    public virtual User User { get; set; } = null!;
}
