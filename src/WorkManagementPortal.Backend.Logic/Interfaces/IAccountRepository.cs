using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.Account;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> CheckExistingEmailAsync(string email);
        Task<UserResponse> RegisterAsync(RegisterModel model);
        Task<UserResponse> LoginAsync(LoginModel model);
        Task<UserResponse> GetCurrentUserAsync(string userId, string token);
        Task<IList<string>> GetRolesAsync(User user);
    }

}
