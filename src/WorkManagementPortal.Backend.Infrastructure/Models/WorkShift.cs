using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Models
{
    public class WorkShift
    {
        public int Id { get; set; }
        public ShiftType ShiftType { get; set; }
        public string ShiftName { get; set; } = string.Empty ;
        public bool IsComplex { get; set; }
        public string ShiftTypeName => ShiftType.ToString();
        public ICollection<User> Users { get; set; } = new HashSet<User>();
        public ICollection<WorkShiftDetail> WorkShiftDetails { get; set; } = new HashSet<WorkShiftDetail>();
    }
}
