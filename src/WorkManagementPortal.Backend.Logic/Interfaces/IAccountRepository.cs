using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.Account;
using WorkManagementPortal.Backend.Infrastructure.Dtos.Account;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> CheckExistingEmailAsync(string email);
        Task<ValidationResponse> RegisterAsync(RegisterModel model);
        Task<LoginValidationResponse> LoginAsync(LoginModel model);
        Task<ValidationResponse> GetCurrentUserAsync(string userId, string token);
        Task<IList<string>> GetRolesAsync(User user);
        Task<ValidationResponse> RequestPasswordResetAsync(string email);
        Task<ValidationResponse> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<ValidationResponse> ResetPasswordAsync(string email, string token, string newPassword);
    }

}
