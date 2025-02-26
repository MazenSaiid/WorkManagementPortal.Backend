using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog
{
    public class WorkTrackingLogDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserDto User { get; set; }
        public DateOnly WorkDate { get; set; }
        public DateTime WorkTimeStart { get; set; }
        public DateTime WorkTimeEnd { get; set; }
        public bool WorkedOvertime { get; set; }
        public bool ExceededPauseHours { get; set; }
        public bool WorkedOutofSchedule { get; set; }
        public bool HasFinished { get; set; }
        public bool IsWorking { get; set; }
        public bool HasCheckedInLate { get; set; }
        public bool HasCheckedOutEarly { get; set; }
        public bool IsPaused { get; set; }
        public List<PauseTrackingLogDTO> PauseTrackingLogs { get; set; }  // Flattened list of PauseTrackingLogs
        public double ActualWorkDurationInHours { get; set; }
        public double OvertimeWorkDurationInHours { get; set; }
    }
}
