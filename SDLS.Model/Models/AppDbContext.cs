using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-silent-recipe-ad74p7pa-pooler.c-2.us-east-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_iFezOVo38tPS;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Answer_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsCorrect).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers).HasConstraintName("Answer_questionId_fkey");
        });

        modelBuilder.Entity<CommentVote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("CommentVote_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ForumComment).WithMany(p => p.CommentVotes).HasConstraintName("CommentVote_forumCommentId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CommentVotes).HasConstraintName("CommentVote_userId_fkey");
        });

        modelBuilder.Entity<DrivingLicense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DrivingLicense_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Exam_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsRandom).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Exam_userId_fkey");
        });

        modelBuilder.Entity<ExamDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamDetail_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Answer).WithMany(p => p.ExamDetails).HasConstraintName("ExamDetail_answerId_fkey");

            entity.HasOne(d => d.ExamSession).WithMany(p => p.ExamDetails).HasConstraintName("ExamDetail_examSessionId_fkey");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamQuestion_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions).HasConstraintName("ExamQuestion_examId_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions).HasConstraintName("ExamQuestion_questionId_fkey");
        });

        modelBuilder.Entity<ExamSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExamSession_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsPassed).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExamSession_examId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ExamSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ExamSession_userId_fkey");
        });

        modelBuilder.Entity<ForumComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ForumComment_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.ForumComments).HasConstraintName("ForumComment_forumPostId_fkey");

            entity.HasOne(d => d.Reply).WithMany(p => p.InverseReply).HasConstraintName("ForumComment_replyId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ForumComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ForumComment_userId_fkey");
        });

        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ForumPost_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.ForumTopic).WithMany(p => p.ForumPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ForumPost_forumTopicId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.ForumPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ForumPost_userId_fkey");
        });

        modelBuilder.Entity<ForumTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ForumTopic_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<LearningProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LearningProgress_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.LearningProgresses).HasConstraintName("LearningProgress_questionId_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.LearningProgress).HasConstraintName("LearningProgress_userId_fkey");
        });

        modelBuilder.Entity<LessonImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LessonImage_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.QuestionLesson).WithMany(p => p.LessonImages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("LessonImage_questionLessonId_fkey");
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LessonProgress_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.QuestionLesson).WithMany(p => p.LessonProgresses).HasConstraintName("LessonProgress_questionLessonId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.LessonProgresses).HasConstraintName("LessonProgress_userId_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notification_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Payment_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_userId_fkey");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostImage_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.PostImages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PostImage_forumPostId_fkey");
        });

        modelBuilder.Entity<PostReact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PostReact_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.PostReacts).HasConstraintName("PostReact_forumPostId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PostReacts).HasConstraintName("PostReact_userId_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Question_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("Question_parentId_fkey");

            entity.HasOne(d => d.QuestionCategory).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Question_questionCategoryId_fkey");

            entity.HasOne(d => d.QuestionLesson).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Question_questionLessonId_fkey");

            entity.HasOne(d => d.QuestionTopic).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Question_questionTopicId_fkey");
        });

        modelBuilder.Entity<QuestionCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionCategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<QuestionChapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionChapter_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.DrivingLicense).WithMany(p => p.QuestionChapters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("QuestionChapter_drivingLicenseId_fkey");
        });

        modelBuilder.Entity<QuestionLesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionLesson_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.QuestionChapter).WithMany(p => p.QuestionLessons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("QuestionLesson_questionChapterId_fkey");
        });

        modelBuilder.Entity<QuestionTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionTag_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionTags).HasConstraintName("QuestionTag_questionId_fkey");

            entity.HasOne(d => d.Tag).WithMany(p => p.QuestionTags).HasConstraintName("QuestionTag_tagId_fkey");
        });

        modelBuilder.Entity<QuestionTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("QuestionTopic_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Report_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ForumComment).WithMany(p => p.Reports).HasConstraintName("Report_forumCommentId_fkey");

            entity.HasOne(d => d.ForumPost).WithMany(p => p.Reports)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Report_forumPostId_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.Reports)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Report_questionId_fkey");

            entity.HasOne(d => d.ReportCategory).WithMany(p => p.Reports).HasConstraintName("Report_reportCategoryId_fkey");

            entity.HasOne(d => d.Simulation).WithMany(p => p.Reports)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Report_simulationId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Report_userId_fkey");
        });

        modelBuilder.Entity<ReportCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ReportCategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Resolve>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Resolve_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Report).WithMany(p => p.Resolves).HasConstraintName("Resolve_reportId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Resolves).HasConstraintName("Resolve_userId_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<SavedQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SavedQuestion_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.SavedQuestions).HasConstraintName("SavedQuestion_questionId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SavedQuestions).HasConstraintName("SavedQuestion_userId_fkey");
        });

        modelBuilder.Entity<SavedTrafficSign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SavedTrafficSign_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.TrafficSign).WithMany(p => p.SavedTrafficSigns).HasConstraintName("SavedTrafficSign_trafficSignId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SavedTrafficSigns).HasConstraintName("SavedTrafficSign_userId_fkey");
        });

        modelBuilder.Entity<SignCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SignCategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<SimulationCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationCategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<SimulationChapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationChapter_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<SimulationDifficultyLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationDifficultyLevel_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<SimulationExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationExam_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Simulation).WithMany(p => p.SimulationExams).HasConstraintName("SimulationExam_simulationId_fkey");

            entity.HasOne(d => d.SituationExam).WithMany(p => p.SimulationExams).HasConstraintName("SimulationExam_situationExamId_fkey");
        });

        modelBuilder.Entity<SimulationScenario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationScenario_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.SimulationCategory).WithMany(p => p.SimulationScenarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationScenario_simulationCategoryId_fkey");

            entity.HasOne(d => d.SimulationChapter).WithMany(p => p.SimulationScenarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationScenario_simulationChapterId_fkey");

            entity.HasOne(d => d.SimulationDifficultyLevel).WithMany(p => p.SimulationScenarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationScenario_simulationDifficultyLevelId_fkey");
        });

        modelBuilder.Entity<SimulationSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationSession_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsPassed).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.SituationExam).WithMany(p => p.SimulationSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationSession_situationExamId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SimulationSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SimulationSession_userId_fkey");
        });

        modelBuilder.Entity<SimulationSessionDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SimulationSessionDetail_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.SimulationExam).WithMany(p => p.SimulationSessionDetails).HasConstraintName("SimulationSessionDetail_simulationExamId_fkey");

            entity.HasOne(d => d.SimulationSession).WithMany(p => p.SimulationSessionDetails).HasConstraintName("SimulationSessionDetail_simulationSessionId_fkey");
        });

        modelBuilder.Entity<SituationExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SituationExam_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsRandom).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tag_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TrafficSign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TrafficSign_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.SignCategory).WithMany(p => p.TrafficSigns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TrafficSign_signCategoryId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_roleId_fkey");
        });

        modelBuilder.Entity<UserLicense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserLicense_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.DrivingLicense).WithMany(p => p.UserLicenses).HasConstraintName("UserLicense_drivingLicenseId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserLicenses).HasConstraintName("UserLicense_userId_fkey");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserNotification_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Notification).WithMany(p => p.UserNotifications).HasConstraintName("UserNotification_notificationId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotifications).HasConstraintName("UserNotification_userId_fkey");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Vehicle_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.DrivingLicense).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Vehicle_drivingLicenseId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
