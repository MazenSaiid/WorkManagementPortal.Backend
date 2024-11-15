using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class UserValidationResponse : ValidationResponse
    {
        public IEnumerable<UserDto> Users { get; set; }
        public UserValidationResponse(bool success, string message, IEnumerable<UserDto> users = null, string token = null)
            : base(success, message, token)
        {
            // If no users are provided, set to an empty list to avoid null reference issues
            Users = users ?? new List<UserDto>();
        }
    }
}
