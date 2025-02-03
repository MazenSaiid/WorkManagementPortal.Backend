using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift
{
    public class ListWorkShiftDto
    {
        public int Id { get; set; }
        public ShiftType ShiftType { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftTypeName => ShiftType.ToString();
        public bool IsComplex { get; set; }
        public List<WorkShiftDetailDto>? WorkShiftDetails { get; set; }
    }
}
