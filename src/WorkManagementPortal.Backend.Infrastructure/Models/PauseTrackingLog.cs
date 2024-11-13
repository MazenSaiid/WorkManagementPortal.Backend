using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class PauseTrackingLog
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }  // Foreign Key to TimeLog
        public WorkTrackingLog WorkTrackingLog { get; set; }

        public PauseType PauseType { get; set; } 
        public DateTime PauseStart { get; set; }
        public DateTime PauseEnd { get; set; }

        public double DurationInMinutes
        {
            get
            {
                // Duration of pause in minutes
                return (PauseEnd - PauseStart).TotalMinutes;
            }
        }
    }


}
