using IdentityService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityService.Data
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<Identity> Users { get; set; }
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
        public IdentityDbContext() { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorizationDbContext).Assembly);
            modelBuilder.Entity<Identity>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<Identity>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Identity>().HasIndex(u => u.Phone).IsUnique();
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=courses_identity_service;Username=postgres;Password=123");
        }
    }
}
