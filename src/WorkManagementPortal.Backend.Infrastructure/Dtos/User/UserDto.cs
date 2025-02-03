using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;

namespace WorkManagementPortal.Backend.API.Dtos.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public int EmployeeSerialNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SupervisorId { get; set; } = string.Empty;
        public UserDto Supervisor { get; set; } 
        public string TeamLeaderId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public UserDto TeamLeader { get; set; }
        public ListWorkShiftDto WorkShift { get; set; }
    }
}
