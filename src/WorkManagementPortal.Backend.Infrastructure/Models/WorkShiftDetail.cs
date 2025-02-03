using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class WorkShiftDetail
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; } // Enum representing the day of the week
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int WorkShiftId { get; set; } // Foreign key to WorkShift
        public WorkShift WorkShift { get; set; } // Navigation property
    }
}
