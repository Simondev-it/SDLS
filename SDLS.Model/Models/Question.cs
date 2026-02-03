using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

[Table("question")]
[Index("Questioncategoryid", Name = "idx_question_category")]
[Index("Questiondifficultylevelid", Name = "idx_question_difficulty")]
public partial class Question
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("questionid")]
    public Guid? Questionid { get; set; }

    [Column("questioncategoryid")]
    public Guid? Questioncategoryid { get; set; }

    [Column("questiondifficultylevelid")]
    public Guid? Questiondifficultylevelid { get; set; }

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("image")]
    public string? Image { get; set; }

    [Column("explanation")]
    public string? Explanation { get; set; }

    [Column("issingleanswer")]
    public bool? Issingleanswer { get; set; }

    [Column("createat", TypeName = "timestamp without time zone")]
    public DateTime? Createat { get; set; }

    [Column("updateat", TypeName = "timestamp without time zone")]
    public DateTime? Updateat { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    [InverseProperty("Question")]
    public virtual ICollection<Eventquestion> Eventquestions { get; set; } = new List<Eventquestion>();

    [InverseProperty("Question")]
    public virtual ICollection<Examdetail> Examdetails { get; set; } = new List<Examdetail>();

    [InverseProperty("QuestionNavigation")]
    public virtual ICollection<Question> InverseQuestionNavigation { get; set; } = new List<Question>();

    [InverseProperty("Question")]
    public virtual ICollection<Learningprogress> Learningprogresses { get; set; } = new List<Learningprogress>();

    [ForeignKey("Questionid")]
    [InverseProperty("InverseQuestionNavigation")]
    public virtual Question? QuestionNavigation { get; set; }

    [ForeignKey("Questioncategoryid")]
    [InverseProperty("Questions")]
    public virtual Questioncategory? Questioncategory { get; set; }

    [ForeignKey("Questiondifficultylevelid")]
    [InverseProperty("Questions")]
    public virtual Questiondifficultylevel? Questiondifficultylevel { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<Questiontag> Questiontags { get; set; } = new List<Questiontag>();

    [InverseProperty("Question")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [InverseProperty("Question")]
    public virtual ICollection<Savedquestion> Savedquestions { get; set; } = new List<Savedquestion>();
}
