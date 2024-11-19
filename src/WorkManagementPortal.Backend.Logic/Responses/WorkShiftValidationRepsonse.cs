using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class WorkShiftValidationRepsonse : ValidationResponse
    {
        public IEnumerable<WorkShift> WorkShifts { get; set; }
        public WorkShiftValidationRepsonse(bool success, string message, IEnumerable<WorkShift> workShifts, string token = null) : base(success, message, token)
        {
            WorkShifts = workShifts;
        }
    }
}
