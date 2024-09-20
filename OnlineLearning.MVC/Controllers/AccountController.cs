using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Service.Interfaces;
using OnlineLearning.Service.ViewModels;
using System.Threading.Tasks;

namespace OnlineLearning.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthUser _authUser;

        public AccountController(IAuthUser authUser)
        {
            _authUser = authUser;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // This will return the registration form view
        }

        // POST: /Account/Register
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> Register(RegisterModelDto model)
        {
            // Check if the model state is valid based on data annotations
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors (e.g., missing fields, invalid email, password mismatch)
                return View(model);
            }

            // Try to register the user with the provided information
            var result = await _authUser.RegisterAsync(model);

            if (result.IsAuthenticated)
            {
                // Registration is successful, redirect the user to login or another action (like dashboard)
                return RedirectToAction("Index", "Home");
            }

            // Registration failed, show the error message returned from the RegisterAsync method
            ModelState.AddModelError(string.Empty, result.Message);

            // Return the view with the error message
            return View(model);
        }


        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // This will return the login form view
        }

        // POST: /Account/Login
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(GetTokenRequestDto model)
        {
            // Ensure the model is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt to authenticate the user and retrieve the token
            var result = await _authUser.GetTokenAsync(model);

            if (result.IsAuthenticated)
            {
                // Store the JWT token securely in the session (optionally encrypted)
                HttpContext.Session.SetString("JWToken", result.Token);

                // Set an authentication cookie with the JWT token
                HttpContext.Response.Cookies.Append("JWToken", result.Token, new CookieOptions
                {
                    HttpOnly = true, // Prevent JavaScript access to cookies (mitigate XSS attacks)
                    Secure = true,   // Ensure the cookie is only sent over HTTPS
                    Expires = DateTime.UtcNow.AddMinutes(30) // Set expiration for the cookie
                });

                // Check for a return URL (if the user was redirected to the login page)
                var returnUrl = Request.Query["ReturnUrl"].FirstOrDefault();

                // Redirect to the return URL if present, otherwise to the homepage
                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            // Authentication failed, add the error message to the ModelState
            ModelState.AddModelError(string.Empty, result.Message);

            // Return the login view with error message(s)
            return View(model);
        }



        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(); // Return view to input email for password reset
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDto email)
        {
            if (string.IsNullOrEmpty(email.Email))
            {
                ModelState.AddModelError("", "Please enter a valid email address.");
                return View();
            }

            var result = await _authUser.ForgetPasswordAsync(email.Email);
            if (result)
            {
                ViewBag.Message = "Password reset link sent successfully!";
            }
            else
            {
                ModelState.AddModelError("", "Error sending password reset link.");
            }

            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordDto { ResetToken = token, Email = email };
            return View(model); // Return view for password reset
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authUser.ResetPasswordAsync(model);
            if (result)
            {
                ViewBag.Message = "Password reset successfully!";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Error resetting password.");
            return View(model);
        }
    }
}
