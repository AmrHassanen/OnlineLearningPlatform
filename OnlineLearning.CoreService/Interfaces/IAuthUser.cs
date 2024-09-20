using OnlineLearning.Entity.Entities;
using OnlineLearning.Service.Authentications;
using OnlineLearning.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearning.Service.Interfaces
{
    public interface IAuthUser
    {
        Task<AppUser> GetCurrentUserAsync();
        Task<OnlineLearningUser> RegisterAsync(RegisterModelDto registerModelDto);
        Task<OnlineLearningUser> GetTokenAsync(GetTokenRequestDto getTokenRequstDto);
        Task<string> AddRoleAsync(RoleModelDto roleModel);
        Task<bool> ForgetPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
