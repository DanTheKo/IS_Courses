using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Data
{
    public class CourseDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseItem> CourseItems { get; set; }
        public DbSet<Content> Contents { get; set; }

/*        public static CourseDbContext Instance => _dbContext ??= new CourseDbContext();
        private static CourseDbContext? _dbContext;*/

        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }
        public CourseDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Каскадное удаление (настраиваем вручную во избежание случайных удалений)
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
