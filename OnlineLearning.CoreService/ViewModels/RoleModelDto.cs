using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Service.ViewModels
{
    public class RoleModelDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

    }
}
