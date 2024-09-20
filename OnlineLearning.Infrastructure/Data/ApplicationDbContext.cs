using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ProgressTracking> ProgressTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SeedRoles(builder);
            base.OnModelCreating(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { ConcurrencyStamp = "1", Name = "Student", NormalizedName = "STUDENT" },
                new IdentityRole { ConcurrencyStamp = "2", Name = "Instructor", NormalizedName = "INSTRUCTOR" },
                new IdentityRole { ConcurrencyStamp = "2", Name = "Admin", NormalizedName = "ADMIN" }
            );
        }
    }
}
