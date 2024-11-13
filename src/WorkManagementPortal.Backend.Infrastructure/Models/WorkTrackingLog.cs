using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class WorkTrackingLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Foreign Key to User
        public User User { get; set; }

        public DateTime ClockIn { get; set; }
        public DateTime ClockOut { get; set; }

        public ICollection<PauseTrackingLog> PauseTrackingLogs { get; set; }  // Collection of pauses

        public double TotalWorkingHours
        {
            get
            {
                // Calculate total working hours, subtracting all pauses
                var totalPauseTime = PauseTrackingLogs?.Sum(p => p.DurationInMinutes) ?? 0;
                return (ClockOut - ClockIn).TotalHours - totalPauseTime / 60;
            }
        }

        public double TotalPausingHours
        {
            get
            {
                // Calculate total pause hours by summing up durations of all pauses
                return (PauseTrackingLogs?.Sum(p => p.DurationInMinutes) ?? 0) / 60;
            }
        }
    }

}
