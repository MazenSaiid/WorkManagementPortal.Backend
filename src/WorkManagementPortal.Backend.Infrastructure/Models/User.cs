using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Foreign Key for the supervisor (Self-Referencing)
        public string? SupervisorId { get; set; }
        public User Supervisor { get; set; }  // Navigation property to Supervisor
        
        // Foreign Key for the team leader (Self-Referencing)
        public string? TeamLeaderId { get; set; }
        public User TeamLeader { get; set; }  // Navigation property to Team Leader

        // Collection of workers supervised by this supervisor
        public ICollection<User> Workers { get; set; } = new HashSet<User>();

        // Collection of supervisors managed by this team leader
        public ICollection<User> Supervisors { get; set; } = new HashSet<User>();
    }

}
