using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyProject.Entities.Entity;

namespace SurveyProject.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TSurvey> TSurveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<AppUser> AppUsers { get; set; } // AppUser sınıfınızı da ekleyin

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TSurvey → Questions (1 → çok)
            modelBuilder.Entity<TSurvey>()
                .HasMany(s => s.Questions)
                .WithOne(q => q.Survey)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question → AnswerOptions (1 → çok)
            modelBuilder.Entity<Question>()
                .HasMany(q => q.AnswerOptions)
                .WithOne(ao => ao.Question)
                .HasForeignKey(ao => ao.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserAnswer → TSurvey (çok → 1)
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Survey)
                .WithMany()
                .HasForeignKey(ua => ua.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserAnswer → Question (çok → 1) — silerken kısıtlama
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserAnswer → AnswerOption (çok → 1) — silerken kısıtlama
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.SelectedOption)
                .WithMany(ao => ao.UserAnswers)
                .HasForeignKey(ua => ua.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserAnswer → AppUser (çok → 1) — kullanıcı silinirse cevapları da silinir
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.AppUser)
                .WithMany(au => au.UserAnswers)
                .HasForeignKey(ua => ua.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }

}

