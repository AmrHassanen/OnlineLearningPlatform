using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using OnlineLearning.Infrastructure.Helpers;
using OnlineLearning.Entity.Entities;
using Microsoft.AspNetCore.Http;
using OnlineLearning.Service.Authentications;
using OnlineLearning.Service.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using OnlineLearning.Service.Interfaces;

namespace Library.Infrastructure.Repositories
{
    public class AuthUser : IAuthUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly ILogger<AuthUser> _logger;

        public AuthUser(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt,
            ILogger<AuthUser> logger,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            var userEmailClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmailClaim))
            {
                return null; // No authenticated user
            }

            return await _userManager.FindByEmailAsync(userEmailClaim);
        }

        public async Task<OnlineLearningUser> RegisterAsync(RegisterModelDto registerModelDto)
        {
            // Check if the email already exists
            var existingEmailUser = await _userManager.FindByEmailAsync(registerModelDto.Email);
            if (existingEmailUser != null)
            {
                return new OnlineLearningUser { Message = "Email is already registered!" }; // Return error message for duplicate email
            }

            // Check if the username already exists
            var existingUsernameUser = await _userManager.FindByNameAsync(registerModelDto.UserName);
            if (existingUsernameUser != null)
            {
                return new OnlineLearningUser { Message = "Username is already registered!" }; // Return error message for duplicate username
            }

            // Create a new user
            var user = new AppUser
            {
                FullName = registerModelDto.FullName,
                Email = registerModelDto.Email,
                UserName = registerModelDto.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerModelDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new OnlineLearningUser { Message = errors }; // Return error messages for failed user creation
            }

            // Assign default role to the user
            await _userManager.AddToRoleAsync(user, "Student");

            // Generate JWT token for the newly registered user
            var jwtSecurityToken = await CreateJwtToken(user);

            return new OnlineLearningUser
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "Student" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
            };
        }


        public async Task<OnlineLearningUser> GetTokenAsync(GetTokenRequestDto getTokenRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(getTokenRequestDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, getTokenRequestDto.Password))
            {
                return new OnlineLearningUser { Message = "Email or password is incorrect." };
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            return new OnlineLearningUser
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                UserName = user.UserName,
                Roles = rolesList.ToList(),
            };
        }

        public async Task<string> AddRoleAsync(RoleModelDto roleModel)
        {
            var user = await _userManager.FindByIdAsync(roleModel.UserId);

            if (user == null || !await _roleManager.RoleExistsAsync(roleModel.Role))
            {
                return "Invalid user ID or role.";
            }

            if (await _userManager.IsInRoleAsync(user, roleModel.Role))
            {
                return "User is already assigned to this role.";
            }

            var result = await _userManager.AddToRoleAsync(user, roleModel.Role);
            return result.Succeeded ? string.Empty : "Something went wrong while assigning the role.";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays((double)_jwt.DurationInDays),
                signingCredentials: signingCredentials);
        }

        public async Task<bool> ForgetPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Include the token in the email message
                    var resetPasswordLink = $"{Uri.EscapeDataString(token)}&email={user.Email}";
                    var messageContent = $"Click <a href='{resetPasswordLink}'>here</a> to reset your password. Your token: {token}";

                    // TODO: Replace the following lines with your email sending logic
                    var message = new MailMessage
                    {
                        From = new MailAddress("amroyasser55555@gmail.com", "Tecical Team"),
                        Subject = "Forget Password Link",
                        Body = messageContent,
                        IsBodyHtml = true
                    };

                    // Set the recipient's email address
                    message.To.Add(new MailAddress(user.Email));

                    // Use your SMTP server details
                    using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new NetworkCredential("amroyasser55555@gmail.com", "vkko ppmn sihc lupe");
                        smtpClient.EnableSsl = true;

                        // Send the email
                        smtpClient.Send(message);
                    }

                    return true; // Password reset link sent successfully
                }

                return false; // User not found or couldn't send the link to the email
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ForgetPasswordAsync: {ex}");
                throw; // You may handle this exception as needed, e.g., log and return a custom response
            }
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return false; // User not found
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.ResetToken, resetPasswordDto.NewPassword);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ResetPasswordAsync: {ex}");
                throw;
            }
        }
    }
}