using AccessService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessService.Data
{
    public class AccessDbContext :DbContext
    {
        public DbSet<Access> Accesses { get; set; }

        public AccessDbContext(DbContextOptions<AccessDbContext> options) : base(options) { }
        public AccessDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Access>()
                .HasIndex(a => new { a.IdentityId, a.ResourceId })
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=courses_access_service;Username=postgres;Password=123");
        }
    }
}
