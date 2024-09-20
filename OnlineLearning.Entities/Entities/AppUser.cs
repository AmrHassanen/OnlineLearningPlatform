using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Entity.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ? Major { get; set; }

        // Navigation properties
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Course> CoursesTaught { get; set; }
    }
}
