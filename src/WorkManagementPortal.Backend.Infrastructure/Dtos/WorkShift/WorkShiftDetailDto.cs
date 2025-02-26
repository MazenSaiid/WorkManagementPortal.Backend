using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift
{
    public class WorkShiftDetailDto
    {
        public int? Id { get; set; }
        public string Day { get; set; } // Day of the week for this work shift detail
        public TimeOnly StartTime { get; set; } // Start time for the specific day
        public TimeOnly EndTime { get; set; } // End time for the specific day
    }
}
