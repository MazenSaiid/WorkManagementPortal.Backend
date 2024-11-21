using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog
{
    // DTO for PauseTrackingLog
    public class PauseTrackingLogDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserDto User { get; set; }
        public DateOnly WorkDate { get; set; }
        public PauseType PauseType { get; set; }
        public string PauseTypeName => PauseType.ToString();
        public int WorkTrackingLogId { get; set; }
        public DateTime PauseStart { get; set; }
        public DateTime? PauseEnd { get; set; }// You can use the enum or a string value
        public double PauseDurationInMinutes { get; set; }
    }
}
