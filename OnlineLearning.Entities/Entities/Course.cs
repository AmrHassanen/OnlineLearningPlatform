using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Entity.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign key for the instructor
        public string InstructorId { get; set; }
        public AppUser Instructor { get; set; }

        // Navigation properties
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
