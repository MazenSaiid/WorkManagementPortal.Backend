using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;  // Foreign Key to User
        public User User { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }  
        public LeaveStatus Status { get; set; } 
    }

}
