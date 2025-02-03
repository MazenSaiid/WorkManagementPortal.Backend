using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift
{
    public class CreateorUpdateWorkShiftDto
    {
        public ShiftType ShiftType { get; set; } // Type of shift (e.g., Morning, Night, etc.)
        public string ShiftName { get; set; } = string.Empty; // Name of the shift
        public bool IsComplex { get; set; } // Flag to indicate if this is a complex work shift
        public List<WorkShiftDetailDto>? WorkShiftDetails { get; set; } // Details for complex shifts (optional for default shifts)
    }
}
