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
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {

            //PrimaryKey
            builder.HasKey(x => x.CourseId);

            // Title validation
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Description validation
            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            // Start and End Date validation
            builder.Property(c => c.StartDate)
                .IsRequired();

            builder.Property(c => c.EndDate)
                .IsRequired();

            // Relationships
            builder.HasOne(c => c.Instructor)
                .WithMany(u => u.CoursesTaught)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId);
        }
    }
}
