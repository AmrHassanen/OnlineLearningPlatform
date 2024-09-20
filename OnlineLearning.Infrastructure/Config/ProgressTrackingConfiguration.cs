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
    public class ProgressTrackingConfiguration : IEntityTypeConfiguration<ProgressTracking>
    {
        public void Configure(EntityTypeBuilder<ProgressTracking> builder)
        {
            // Primary Key
            builder.HasKey(p => p.ProgressTrackingId);

            // Completion Percentage validation
            builder.Property(p => p.CompletionPercentage)
                .IsRequired()
                .HasDefaultValue(0);

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.Course)
                .WithMany()
                .HasForeignKey(p => p.CourseId);
        }
    }
}
