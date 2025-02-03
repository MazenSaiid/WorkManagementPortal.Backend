using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class ApplicationActivityLog
    {
        public int Id { get; set; }
        public string ApplicationName { get; set; }
        public string Website { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double DurationInSeconds { get; set; }
        public string UserId { get; set; }  // Foreign Key to User
        public User User { get; set; }
    }
}
