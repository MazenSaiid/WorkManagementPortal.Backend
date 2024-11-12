using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class Pause
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }  // Foreign Key to TimeLog
        public WorkLog WorkLog { get; set; }

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
