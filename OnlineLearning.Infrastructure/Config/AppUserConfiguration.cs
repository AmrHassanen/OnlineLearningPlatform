using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Infrastructure.Config
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // Full Name validation
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasMany(u => u.CoursesTaught)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Enrollments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);
        }
    }
}
