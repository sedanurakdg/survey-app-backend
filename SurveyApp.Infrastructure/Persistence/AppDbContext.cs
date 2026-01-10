using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Core.Entities;
using SurveyApp.Core.Identity;

namespace SurveyApp.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AnswerTemplate> AnswerTemplates => Set<AnswerTemplate>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<SurveyQuestion> SurveyQuestions => Set<SurveyQuestion>();
    public DbSet<SurveyAssignment> SurveyAssignments => Set<SurveyAssignment>();
    public DbSet<SurveySubmission> SurveySubmissions => Set<SurveySubmission>();
    public DbSet<SurveySubmissionAnswer> SurveySubmissionAnswers => Set<SurveySubmissionAnswer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AnswerTemplate>(e =>
        {
            e.ToTable("AnswerTemplates", "survey");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.IsActive).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired();

            e.HasMany(x => x.Options)
             .WithOne(x => x.AnswerTemplate)
             .HasForeignKey(x => x.AnswerTemplateId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AnswerOption>(e =>
        {
            e.ToTable("AnswerOptions", "survey");
            e.HasKey(x => x.Id);

            e.Property(x => x.Text).HasMaxLength(200).IsRequired();
            e.Property(x => x.SortOrder).IsRequired();

            e.HasIndex(x => new { x.AnswerTemplateId, x.SortOrder }).IsUnique();
        });

        builder.Entity<Question>(e =>
        {
            e.ToTable("Questions", "survey");
            e.HasKey(x => x.Id);

            e.Property(x => x.Text).HasMaxLength(1000).IsRequired();
            e.Property(x => x.IsActive).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired();

            e.HasOne(x => x.AnswerTemplate)
             .WithMany()
             .HasForeignKey(x => x.AnswerTemplateId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.AnswerTemplateId);
        });
        builder.Entity<Survey>(e =>
        {
            e.ToTable("Surveys", "survey");
            e.HasKey(x => x.Id);

            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.StartsAtUtc).IsRequired();
            e.Property(x => x.EndsAtUtc).IsRequired();
            e.Property(x => x.IsActive).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired();

            e.HasIndex(x => x.IsActive);
        });

        builder.Entity<SurveyQuestion>(e =>
        {
            e.ToTable("SurveyQuestions", "survey");
            e.HasKey(x => new { x.SurveyId, x.QuestionId });

            e.Property(x => x.SortOrder).IsRequired();

            e.HasOne(x => x.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(x => x.SurveyId);

            e.HasOne(x => x.Question)
                .WithMany()
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.SurveyId, x.SortOrder }).IsUnique();
        });

        builder.Entity<SurveyAssignment>(e =>
        {
            e.ToTable("SurveyAssignments", "survey");
            e.HasKey(x => new { x.SurveyId, x.UserId });

            e.HasOne(x => x.Survey)
                .WithMany(s => s.Assignments)
                .HasForeignKey(x => x.SurveyId);

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SurveySubmission>(e =>
        {
            e.ToTable("SurveySubmissions", "survey");
            e.HasKey(x => x.Id);

            e.Property(x => x.SubmittedAtUtc).IsRequired();
            e.HasIndex(x => new { x.SurveyId, x.UserId }).IsUnique();

            e.HasOne(x => x.Survey)
                .WithMany()
                .HasForeignKey(x => x.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(x => x.Answers)
                .WithOne(a => a.Submission)
                .HasForeignKey(a => a.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SurveySubmissionAnswer>(e =>
        {
            e.ToTable("SurveySubmissionAnswers", "survey");
            e.HasKey(x => new { x.SubmissionId, x.QuestionId });

            e.Property(x => x.SelectedOptionIndex).IsRequired();

            e.HasOne(x => x.Question)
                .WithMany()
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }

}

