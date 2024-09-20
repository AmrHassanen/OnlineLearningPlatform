using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Entity.Entities
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; }

        // Foreign keys
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
