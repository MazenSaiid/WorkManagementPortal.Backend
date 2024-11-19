using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class PauseTrackingLog
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }  // Foreign Key to WorkTrackingLog
        public WorkTrackingLog WorkTrackingLog { get; set; }

        public string UserId { get; set; }  // User ID for easier querying of pauses
        public PauseType PauseType { get; set; }  // Type of pause (e.g., "Lunch", "Break")
        public DateTime PauseStart { get; set; }
        public DateTime PauseEnd { get; set; }
        public double PauseDuration { get; set; }

    }

}
