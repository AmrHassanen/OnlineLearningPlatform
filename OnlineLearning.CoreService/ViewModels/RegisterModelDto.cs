using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Service.ViewModels
{
        public class RegisterModelDto
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username must be between {2} and {1} characters.", MinimumLength = 3)]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username must be between {2} and {1} characters.", MinimumLength = 3)]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [StringLength(128, ErrorMessage = "Email must not exceed {1} characters.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(256, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required.")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
}

