using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class LoginValidationResponse :ValidationResponse
    {
        public LoginValidationResponse(bool success, string message, string token = null, string username = null, string userId = null, List<string> roles = null, DateTime localSessionExpireDate = default) : base(success, message, token)
        {
            Username = username;
            UserId = userId;
            Roles = roles;
            LocalSessionExpiryDate = localSessionExpireDate;
        }

        public string UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public DateTime LocalSessionExpiryDate { get; set; }

    }
}
