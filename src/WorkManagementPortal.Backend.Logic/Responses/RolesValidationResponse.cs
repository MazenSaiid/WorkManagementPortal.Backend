using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.Roles;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class RolesValidationResponse : ValidationResponse
    {
        public IEnumerable<RolesListDto> Roles { get; set; }

        public RolesValidationResponse(
            bool success,
            string message,
            IEnumerable<RolesListDto> roles = null,
            string token = null)
            : base(success, message, token)
        {
            Roles = roles ?? new List<RolesListDto>();
        }
    }
}
