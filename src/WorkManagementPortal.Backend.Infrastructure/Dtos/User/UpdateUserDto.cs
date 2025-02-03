using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.User
{
    public class UpdateUserDto
    {
        public int EmployeeSerialNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? SupervisorId { get; set; } 
        public string? TeamLeaderId { get; set; }
        public int? WorkShiftId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
