using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace SDLS.Model.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<CommentVote> CommentVotes { get; set; }

    public virtual DbSet<DrivingLicense> DrivingLicenses { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamDetail> ExamDetails { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<ExamSession> ExamSessions { get; set; }

    public virtual DbSet<ForumComment> ForumComments { get; set; }

    public virtual DbSet<ForumPost> ForumPosts { get; set; }

    public virtual DbSet<ForumTopic> ForumTopics { get; set; }

    public virtual DbSet<LearningProgress> LearningProgresses { get; set; }

    public virtual DbSet<LessonImage> LessonImages { get; set; }

    public virtual DbSet<LessonProgress> LessonProgresses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PostImage> PostImages { get; set; }

    public virtual DbSet<PostReact> PostReacts { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionCategory> QuestionCategories { get; set; }

    public virtual DbSet<QuestionChapter> QuestionChapters { get; set; }

    public virtual DbSet<QuestionLesson> QuestionLessons { get; set; }

    public virtual DbSet<QuestionTag> QuestionTags { get; set; }

    public virtual DbSet<QuestionTopic> QuestionTopics { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportCategory> ReportCategories { get; set; }

    public virtual DbSet<Resolve> Resolves { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedQuestion> SavedQuestions { get; set; }

    public virtual DbSet<SavedTrafficSign> SavedTrafficSigns { get; set; }

    public virtual DbSet<SignCategory> SignCategories { get; set; }

    public virtual DbSet<SimulationCategory> SimulationCategories { get; set; }

    public virtual DbSet<SimulationChapter> SimulationChapters { get; set; }

    public virtual DbSet<SimulationDifficultyLevel> SimulationDifficultyLevels { get; set; }

    public virtual DbSet<SimulationExam> SimulationExams { get; set; }

    public virtual DbSet<SimulationScenario> SimulationScenarios { get; set; }

    public virtual DbSet<SimulationSession> SimulationSessions { get; set; }

    public virtual DbSet<SimulationSessionDetail> SimulationSessionDetails { get; set; }

    public virtual DbSet<SituationExam> SituationExams { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TrafficSign> TrafficSigns { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLicense> UserLicenses { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseNpgsql("Host=ep-silent-recipe-ad74p7pa-pooler.c-2.us-east-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_iFezOVo38tPS;SSL Mode=Require;Trust Server Certificate=true");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Answer_pkey");

            entity.ToTable("Answer");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(true)
                .HasColumnName("isCorrect");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("Answer_questionId_fkey");
        });

        modelBuilder.Entity<CommentVote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CommentVote_pkey");

            entity.ToTable("CommentVote");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ForumCommentId).HasColumnName("forumCommentId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.ForumComment).WithMany(p => p.CommentVotes)
                .HasForeignKey(d => d.ForumCommentId)
                .HasConstraintName("CommentVote_forumCommentId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CommentVotes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("CommentVote_userId_fkey");
        });

        modelBuilder.Entity<DrivingLicense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DrivingLicense_pkey");

            entity.ToTable("DrivingLicense");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Exam_pkey");

            entity.ToTable("Exam");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.IsRandom)
                .HasDefaultValue(false)
                .HasColumnName("isRandom");
            entity.Property(e => e.PassScore).HasColumnName("passScore");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Exams)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Exam_userId_fkey");
        });

        modelBuilder.Entity<ExamDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamDetail_pkey");

            entity.ToTable("ExamDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AnswerId).HasColumnName("answerId");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ExamSessionId).HasColumnName("examSessionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Answer).WithMany(p => p.ExamDetails)
                .HasForeignKey(d => d.AnswerId)
                .HasConstraintName("ExamDetail_answerId_fkey");

            entity.HasOne(d => d.ExamSession).WithMany(p => p.ExamDetails)
                .HasForeignKey(d => d.ExamSessionId)
                .HasConstraintName("ExamDetail_examSessionId_fkey");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamQuestion_pkey");

            entity.ToTable("ExamQuestion");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ExamId).HasColumnName("examId");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("ExamQuestion_examId_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("ExamQuestion_questionId_fkey");
        });

        modelBuilder.Entity<ExamSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamSession_pkey");

            entity.ToTable("ExamSession");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ExamId).HasColumnName("examId");
            entity.Property(e => e.IsPassed)
                .HasDefaultValue(true)
                .HasColumnName("isPassed");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TotalDuration).HasColumnName("totalDuration");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExamSession_examId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ExamSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExamSession_userId_fkey");
        });

        modelBuilder.Entity<ForumComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ForumComment_pkey");

            entity.ToTable("ForumComment");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ForumPostId).HasColumnName("forumPostId");
            entity.Property(e => e.ReplyId).HasColumnName("replyId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.ForumComments)
                .HasForeignKey(d => d.ForumPostId)
                .HasConstraintName("ForumComment_forumPostId_fkey");

            entity.HasOne(d => d.Reply).WithMany(p => p.InverseReply)
                .HasForeignKey(d => d.ReplyId)
                .HasConstraintName("ForumComment_replyId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ForumComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ForumComment_userId_fkey");
        });

        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ForumPost_pkey");

            entity.ToTable("ForumPost");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ForumTopicId).HasColumnName("forumTopicId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.ViewCount)
                .HasDefaultValue(0)
                .HasColumnName("viewCount");

            entity.HasOne(d => d.ForumTopic).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.ForumTopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ForumPost_forumTopicId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ForumPost_userId_fkey");
        });

        modelBuilder.Entity<ForumTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ForumTopic_pkey");

            entity.ToTable("ForumTopic");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<LearningProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LearningProgress_pkey");

            entity.ToTable("LearningProgress");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Question).WithMany(p => p.LearningProgresses)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("LearningProgress_questionId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.LearningProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("LearningProgress_userId_fkey");
        });

        modelBuilder.Entity<LessonImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LessonImage_pkey");

            entity.ToTable("LessonImage");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.QuestionLessonId).HasColumnName("questionLessonId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.QuestionLesson).WithMany(p => p.LessonImages)
                .HasForeignKey(d => d.QuestionLessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("LessonImage_questionLessonId_fkey");
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LessonProgress_pkey");

            entity.ToTable("LessonProgress");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.QuestionLessonId).HasColumnName("questionLessonId");
            entity.Property(e => e.Score)
                .HasDefaultValue(0)
                .HasColumnName("score");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.QuestionLesson).WithMany(p => p.LessonProgresses)
                .HasForeignKey(d => d.QuestionLessonId)
                .HasConstraintName("LessonProgress_questionLessonId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.LessonProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("LessonProgress_userId_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notification_pkey");

            entity.ToTable("Notification");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Payment_pkey");

            entity.ToTable("Payment");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Method)
                .HasMaxLength(255)
                .HasColumnName("method");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.OrderCode).HasColumnName("orderCode");
            entity.Property(e => e.Response)
                .HasMaxLength(255)
                .HasColumnName("response");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_userId_fkey");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostImage_pkey");

            entity.ToTable("PostImage");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ForumPostId).HasColumnName("forumPostId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.PostImages)
                .HasForeignKey(d => d.ForumPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostImage_forumPostId_fkey");
        });

        modelBuilder.Entity<PostReact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostReact_pkey");

            entity.ToTable("PostReact");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ForumPostId).HasColumnName("forumPostId");
            entity.Property(e => e.ReactType)
                .HasMaxLength(20)
                .HasColumnName("reactType");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.PostReacts)
                .HasForeignKey(d => d.ForumPostId)
                .HasConstraintName("PostReact_forumPostId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PostReacts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("PostReact_userId_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Question_pkey");

            entity.ToTable("Question");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Explanation)
                .HasMaxLength(255)
                .HasColumnName("explanation");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.ParentId).HasColumnName("parentId");
            entity.Property(e => e.QuestionCategoryId).HasColumnName("questionCategoryId");
            entity.Property(e => e.QuestionLessonId).HasColumnName("questionLessonId");
            entity.Property(e => e.QuestionTopicId).HasColumnName("questionTopicId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("Question_parentId_fkey");

            entity.HasOne(d => d.QuestionCategory).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuestionCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Question_questionCategoryId_fkey");

            entity.HasOne(d => d.QuestionLesson).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuestionLessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Question_questionLessonId_fkey");

            entity.HasOne(d => d.QuestionTopic).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuestionTopicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Question_questionTopicId_fkey");
        });

        modelBuilder.Entity<QuestionCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionCategory_pkey");

            entity.ToTable("QuestionCategory");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<QuestionChapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionChapter_pkey");

            entity.ToTable("QuestionChapter");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DrivingLicenseId).HasColumnName("drivingLicenseId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.DrivingLicense).WithMany(p => p.QuestionChapters)
                .HasForeignKey(d => d.DrivingLicenseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("QuestionChapter_drivingLicenseId_fkey");
        });

        modelBuilder.Entity<QuestionLesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionLesson_pkey");

            entity.ToTable("QuestionLesson");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.QuestionChapterId).HasColumnName("questionChapterId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.QuestionChapter).WithMany(p => p.QuestionLessons)
                .HasForeignKey(d => d.QuestionChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("QuestionLesson_questionChapterId_fkey");
        });

        modelBuilder.Entity<QuestionTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionTag_pkey");

            entity.ToTable("QuestionTag");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TagId).HasColumnName("tagId");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionTags)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("QuestionTag_questionId_fkey");

            entity.HasOne(d => d.Tag).WithMany(p => p.QuestionTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("QuestionTag_tagId_fkey");
        });

        modelBuilder.Entity<QuestionTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionTopic_pkey");

            entity.ToTable("QuestionTopic");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Report_pkey");

            entity.ToTable("Report");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ForumCommentId).HasColumnName("forumCommentId");
            entity.Property(e => e.ForumPostId).HasColumnName("forumPostId");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.ReportCategoryId).HasColumnName("reportCategoryId");
            entity.Property(e => e.SimulationId).HasColumnName("simulationId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.ForumComment).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ForumCommentId)
                .HasConstraintName("Report_forumCommentId_fkey");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ForumPostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Report_forumPostId_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.Reports)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Report_questionId_fkey");

            entity.HasOne(d => d.ReportCategory).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ReportCategoryId)
                .HasConstraintName("Report_reportCategoryId_fkey");

            entity.HasOne(d => d.Simulation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.SimulationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Report_simulationId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Report_userId_fkey");
        });

        modelBuilder.Entity<ReportCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ReportCategory_pkey");

            entity.ToTable("ReportCategory");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Resolve>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Resolve_pkey");

            entity.ToTable("Resolve");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.ReportId).HasColumnName("reportId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Report).WithMany(p => p.Resolves)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("Resolve_reportId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Resolves)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Resolve_userId_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<SavedQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SavedQuestion_pkey");

            entity.ToTable("SavedQuestion");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.QuestionId).HasColumnName("questionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Question).WithMany(p => p.SavedQuestions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("SavedQuestion_questionId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SavedQuestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("SavedQuestion_userId_fkey");
        });

        modelBuilder.Entity<SavedTrafficSign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SavedTrafficSign_pkey");

            entity.ToTable("SavedTrafficSign");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TrafficSignId).HasColumnName("trafficSignId");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.TrafficSign).WithMany(p => p.SavedTrafficSigns)
                .HasForeignKey(d => d.TrafficSignId)
                .HasConstraintName("SavedTrafficSign_trafficSignId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SavedTrafficSigns)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("SavedTrafficSign_userId_fkey");
        });

        modelBuilder.Entity<SignCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SignCategory_pkey");

            entity.ToTable("SignCategory");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<SimulationCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationCategory_pkey");

            entity.ToTable("SimulationCategory");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<SimulationChapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationChapter_pkey");

            entity.ToTable("SimulationChapter");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<SimulationDifficultyLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationDifficultyLevel_pkey");

            entity.ToTable("SimulationDifficultyLevel");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<SimulationExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationExam_pkey");

            entity.ToTable("SimulationExam");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BaseScore).HasColumnName("baseScore");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.SimulationId).HasColumnName("simulationId");
            entity.Property(e => e.SituationExamId).HasColumnName("situationExamId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Simulation).WithMany(p => p.SimulationExams)
                .HasForeignKey(d => d.SimulationId)
                .HasConstraintName("SimulationExam_simulationId_fkey");

            entity.HasOne(d => d.SituationExam).WithMany(p => p.SimulationExams)
                .HasForeignKey(d => d.SituationExamId)
                .HasConstraintName("SimulationExam_situationExamId_fkey");
        });

        modelBuilder.Entity<SimulationScenario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationScenario_pkey");

            entity.ToTable("SimulationScenario");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EndPoint).HasColumnName("endPoint");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.SimulationCategoryId).HasColumnName("simulationCategoryId");
            entity.Property(e => e.SimulationChapterId).HasColumnName("simulationChapterId");
            entity.Property(e => e.SimulationDifficultyLevelId).HasColumnName("simulationDifficultyLevelId");
            entity.Property(e => e.StartPoint).HasColumnName("startPoint");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TotalTime).HasColumnName("totalTime");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.Video)
                .HasMaxLength(255)
                .HasColumnName("video");

            entity.HasOne(d => d.SimulationCategory).WithMany(p => p.SimulationScenarios)
                .HasForeignKey(d => d.SimulationCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationScenario_simulationCategoryId_fkey");

            entity.HasOne(d => d.SimulationChapter).WithMany(p => p.SimulationScenarios)
                .HasForeignKey(d => d.SimulationChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationScenario_simulationChapterId_fkey");

            entity.HasOne(d => d.SimulationDifficultyLevel).WithMany(p => p.SimulationScenarios)
                .HasForeignKey(d => d.SimulationDifficultyLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationScenario_simulationDifficultyLevelId_fkey");
        });

        modelBuilder.Entity<SimulationSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationSession_pkey");

            entity.ToTable("SimulationSession");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.IsPassed)
                .HasDefaultValue(true)
                .HasColumnName("isPassed");
            entity.Property(e => e.SituationExamId).HasColumnName("situationExamId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.TotalDuration).HasColumnName("totalDuration");
            entity.Property(e => e.TotalScore).HasColumnName("totalScore");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.SituationExam).WithMany(p => p.SimulationSessions)
                .HasForeignKey(d => d.SituationExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationSession_situationExamId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SimulationSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationSession_userId_fkey");
        });

        modelBuilder.Entity<SimulationSessionDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationSessionDetail_pkey");

            entity.ToTable("SimulationSessionDetail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.DurationSecond).HasColumnName("durationSecond");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.SimulationExamId).HasColumnName("simulationExamId");
            entity.Property(e => e.SimulationSessionId).HasColumnName("simulationSessionId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.SimulationExam).WithMany(p => p.SimulationSessionDetails)
                .HasForeignKey(d => d.SimulationExamId)
                .HasConstraintName("SimulationSessionDetail_simulationExamId_fkey");

            entity.HasOne(d => d.SimulationSession).WithMany(p => p.SimulationSessionDetails)
                .HasForeignKey(d => d.SimulationSessionId)
                .HasConstraintName("SimulationSessionDetail_simulationSessionId_fkey");
        });

        modelBuilder.Entity<SituationExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SituationExam_pkey");

            entity.ToTable("SituationExam");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.IsRandom)
                .HasDefaultValue(false)
                .HasColumnName("isRandom");
            entity.Property(e => e.PassScore).HasColumnName("passScore");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tag_pkey");

            entity.ToTable("Tag");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ColorCode)
                .HasMaxLength(255)
                .HasColumnName("colorCode");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<TrafficSign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TrafficSign_pkey");

            entity.ToTable("TrafficSign");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.SignCategoryId).HasColumnName("signCategoryId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.VectorData)
                .HasMaxLength(255)
                .HasColumnName("vectorData");

            entity.HasOne(d => d.SignCategory).WithMany(p => p.TrafficSigns)
                .HasForeignKey(d => d.SignCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrafficSign_signCategoryId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .HasColumnName("gender");
            entity.Property(e => e.LicenseType)
                .HasMaxLength(20)
                .HasColumnName("licenseType");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_roleId_fkey");
        });

        modelBuilder.Entity<UserLicense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserLicense_pkey");

            entity.ToTable("UserLicense");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.DrivingLicenseId).HasColumnName("drivingLicenseId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.DrivingLicense).WithMany(p => p.UserLicenses)
                .HasForeignKey(d => d.DrivingLicenseId)
                .HasConstraintName("UserLicense_drivingLicenseId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserLicenses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserLicense_userId_fkey");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserNotification_pkey");

            entity.ToTable("UserNotification");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.NotificationId).HasColumnName("notificationId");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Notification).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.NotificationId)
                .HasConstraintName("UserNotification_notificationId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserNotification_userId_fkey");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Vehicle_pkey");

            entity.ToTable("Vehicle");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DrivingLicenseId).HasColumnName("drivingLicenseId");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.DrivingLicense).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.DrivingLicenseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Vehicle_drivingLicenseId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
