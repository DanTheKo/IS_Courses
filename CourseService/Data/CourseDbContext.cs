using CourseService.Models;
using CourseService.Models.Quizes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Data
{
    public class CourseDbContext : DbContext
    {
        //Course
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseItem> CourseItems { get; set; }
        public DbSet<Content> Contents { get; set; }
        //Interactive
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizResponse> QuizResponses { get; set; }
        public DbSet<Question> Questions{ get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<Feedback> Feedbacks{ get; set; }

        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }
        public CourseDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<CourseItem>()
                .HasOne(m => m.Course)
                .WithMany(c => c.CourseItems)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Content>()
                .HasOne(b => b.CourseItem)
                .WithMany(p => p.Contents)
                .OnDelete(DeleteBehavior.Cascade);
            */
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=courses_courses_service;Username=postgres;Password=123");
        }
    }
}
