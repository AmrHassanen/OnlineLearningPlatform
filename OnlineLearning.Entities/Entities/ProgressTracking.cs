using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Entity.Entities
{
    public class ProgressTracking
    {
        public int ProgressTrackingId { get; set; }
        public int CompletionPercentage { get; set; }

        // Foreign keys
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
