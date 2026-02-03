using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SDLS.Model.Models;

public partial class SdlsDbContext : DbContext
{
    public SdlsDbContext()
    {
    }

    public SdlsDbContext(DbContextOptions<SdlsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Eventquestion> Eventquestions { get; set; }

    public virtual DbSet<Eventsession> Eventsessions { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Examdetail> Examdetails { get; set; }

    public virtual DbSet<Examsession> Examsessions { get; set; }

    public virtual DbSet<Forumcomment> Forumcomments { get; set; }

    public virtual DbSet<Forumpost> Forumposts { get; set; }

    public virtual DbSet<Forumtopic> Forumtopics { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<Jwk> Jwks { get; set; }

    public virtual DbSet<Learningprogress> Learningprogresses { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PlayingWithNeon> PlayingWithNeons { get; set; }

    public virtual DbSet<ProjectConfig> ProjectConfigs { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Questioncategory> Questioncategories { get; set; }

    public virtual DbSet<Questiondifficultylevel> Questiondifficultylevels { get; set; }

    public virtual DbSet<Questiontag> Questiontags { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Reportcategory> Reportcategories { get; set; }

    public virtual DbSet<Resolve> Resolves { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rolepermission> Rolepermissions { get; set; }

    public virtual DbSet<Savedquestion> Savedquestions { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Simulationcategory> Simulationcategories { get; set; }

    public virtual DbSet<Simulationchapter> Simulationchapters { get; set; }

    public virtual DbSet<Simulationdifficultylevel> Simulationdifficultylevels { get; set; }

    public virtual DbSet<Simulationscenario> Simulationscenarios { get; set; }

    public virtual DbSet<Simulationsession> Simulationsessions { get; set; }

    public virtual DbSet<Starcategory> Starcategories { get; set; }

    public virtual DbSet<Trafficsign> Trafficsigns { get; set; }

    public virtual DbSet<Trafficsigncategory> Trafficsigncategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<User1> Users1 { get; set; }

    public virtual DbSet<Usernotification> Usernotifications { get; set; }

    public virtual DbSet<Verification> Verifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-silent-water-a15kqusf-pooler.ap-southeast-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_YTl1rgocn7XU;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_session_jwt");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts).HasConstraintName("account_userId_fkey");
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("answer_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Iscorrect).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("answer_questionid_fkey");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("event_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Eventquestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventquestion_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Event).WithMany(p => p.Eventquestions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("eventquestion_eventid_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.Eventquestions).HasConstraintName("eventquestion_questionid_fkey");
        });

        modelBuilder.Entity<Eventsession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventsession_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Event).WithMany(p => p.Eventsessions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("eventsession_eventid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Eventsessions).HasConstraintName("eventsession_userid_fkey");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("exam_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Owndone).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Exams).HasConstraintName("exam_userid_fkey");
        });

        modelBuilder.Entity<Examdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("examdetail_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Examsession).WithMany(p => p.Examdetails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examdetail_examsessionid_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.Examdetails).HasConstraintName("examdetail_questionid_fkey");
        });

        modelBuilder.Entity<Examsession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("examsession_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Ispassed).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Exam).WithMany(p => p.Examsessions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("examsession_examid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Examsessions).HasConstraintName("examsession_userid_fkey");
        });

        modelBuilder.Entity<Forumcomment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("forumcomment_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Forumpost).WithMany(p => p.Forumcomments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("forumcomment_forumpostid_fkey");

            entity.HasOne(d => d.Reply).WithMany(p => p.InverseReply).HasConstraintName("forumcomment_replyid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Forumcomments).HasConstraintName("forumcomment_userid_fkey");
        });

        modelBuilder.Entity<Forumpost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("forumpost_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Viewcount).HasDefaultValue(0);

            entity.HasOne(d => d.Forumtopic).WithMany(p => p.Forumposts).HasConstraintName("forumpost_forumtopicid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Forumposts).HasConstraintName("forumpost_userid_fkey");
        });

        modelBuilder.Entity<Forumtopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("forumtopic_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invitation_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Inviter).WithMany(p => p.Invitations).HasConstraintName("invitation_inviterId_fkey");

            entity.HasOne(d => d.Organization).WithMany(p => p.Invitations).HasConstraintName("invitation_organizationId_fkey");
        });

        modelBuilder.Entity<Jwk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jwks_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<Learningprogress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("learningprogress_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.Learningprogresses).HasConstraintName("learningprogress_questionid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Learningprogresses)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("learningprogress_userid_fkey");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("member_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Organization).WithMany(p => p.Members).HasConstraintName("member_organizationId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Members).HasConstraintName("member_userId_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organization_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payment_userid_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permission_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<PlayingWithNeon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("playing_with_neon_pkey");
        });

        modelBuilder.Entity<ProjectConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("project_config_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("question_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Issingleanswer).HasDefaultValue(true);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Questioncategory).WithMany(p => p.Questions).HasConstraintName("question_questioncategoryid_fkey");

            entity.HasOne(d => d.Questiondifficultylevel).WithMany(p => p.Questions).HasConstraintName("question_questiondifficultylevelid_fkey");

            entity.HasOne(d => d.QuestionNavigation).WithMany(p => p.InverseQuestionNavigation).HasConstraintName("question_questionid_fkey");
        });

        modelBuilder.Entity<Questioncategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questioncategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Questiondifficultylevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questiondifficultylevel_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Questiontag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questiontag_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.Questiontags)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("questiontag_questionid_fkey");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("report_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.Reports).HasConstraintName("report_questionid_fkey");

            entity.HasOne(d => d.Reportcategory).WithMany(p => p.Reports).HasConstraintName("report_reportcategoryid_fkey");

            entity.HasOne(d => d.Simulation).WithMany(p => p.Reports).HasConstraintName("report_simulationid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reports).HasConstraintName("report_userid_fkey");
        });

        modelBuilder.Entity<Reportcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reportcategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Resolve>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("resolve_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Report).WithMany(p => p.Resolves)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("resolve_reportid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Rolepermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rolepermission_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Permission).WithMany(p => p.Rolepermissions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rolepermission_permissionid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Rolepermissions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rolepermission_roleid_fkey");
        });

        modelBuilder.Entity<Savedquestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("savedquestion_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Question).WithMany(p => p.Savedquestions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("savedquestion_questionid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Savedquestions)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("savedquestion_userid_fkey");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("session_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions).HasConstraintName("session_userId_fkey");
        });

        modelBuilder.Entity<Simulationcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("simulationcategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Simulationchapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("simulationchapter_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Simulationdifficultylevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("simulationdifficultylevel_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Simulationscenario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("simulationscenario_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Simulationcategory).WithMany(p => p.Simulationscenarios).HasConstraintName("simulationscenario_simulationcategoryid_fkey");

            entity.HasOne(d => d.Simulationchapter).WithMany(p => p.Simulationscenarios).HasConstraintName("simulationscenario_simulationchapterid_fkey");

            entity.HasOne(d => d.Simulationdifficultylevel).WithMany(p => p.Simulationscenarios).HasConstraintName("simulationscenario_simulationdifficultylevelid_fkey");
        });

        modelBuilder.Entity<Simulationsession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("simulationsession_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Ispassed).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Simulation).WithMany(p => p.Simulationsessions).HasConstraintName("simulationsession_simulationid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Simulationsessions).HasConstraintName("simulationsession_userid_fkey");
        });

        modelBuilder.Entity<Starcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("starcategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Trafficsign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trafficsign_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Trafficsigncategory).WithMany(p => p.Trafficsigns).HasConstraintName("trafficsign_trafficsigncategoryid_fkey");
        });

        modelBuilder.Entity<Trafficsigncategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trafficsigncategory_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Starcategory).WithMany(p => p.Trafficsigncategories).HasConstraintName("trafficsigncategory_starcategoryid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Point).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users).HasConstraintName("User_roleid_fkey");
        });

        modelBuilder.Entity<User1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Usernotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usernotification_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Createat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.Updateat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Notification).WithMany(p => p.Usernotifications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usernotification_notificationid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Usernotifications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usernotification_userid_fkey");
        });

        modelBuilder.Entity<Verification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("verification_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
