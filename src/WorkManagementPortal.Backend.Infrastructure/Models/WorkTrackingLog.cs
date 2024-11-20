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

        public DateOnly WorkDate { get; set; }
        public DateTime WorkTimeStart { get; set; }
        public DateTime WorkTimeEnd { get; set; }

        public bool IsWorking { get; set; }
        public bool IsPaused { get; set; }
        public bool IsFinished { get; set; }
        public ICollection<PauseTrackingLog> PauseTrackingLogs { get; set; }  // Collection of pauses
        public double ActualWorkDuration { get; set; }
    }

}
