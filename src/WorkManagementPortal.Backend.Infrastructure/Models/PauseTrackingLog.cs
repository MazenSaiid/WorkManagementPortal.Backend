using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class PauseTrackingLog
    {
        public int Id { get; set; }
        public int WorkTrackingLogId { get; set; }  // Foreign Key
        public WorkTrackingLog WorkTrackingLog { get; set; }  // Navigation property

        public string UserId { get; set; }  // User ID for easier querying of pauses
        public DateOnly WorkDate { get; set; }
        public PauseType PauseType { get; set; }  // Type of pause (e.g., "Lunch", "Break")
        public DateTime PauseStart { get; set; }
        public DateTime PauseEnd { get; set; }
        public double PauseDurationInMinutes { get; set; }
    }
    
}
