using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.Roles
{
    public class RoleUpdateDto
    {
        public string RoleId { get; set; }
        public string NewRoleName { get; set; }
    }
}
