using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class WorkShiftValidationRepsonse : ValidationResponse
    {
        public IEnumerable<ListWorkShiftDto> WorkShifts { get; set; }

        public WorkShiftValidationRepsonse(bool success, string message, string token = null, IEnumerable<ListWorkShiftDto> workShifts = null) : base(success, message, token)
        {
            WorkShifts = workShifts ?? new List<ListWorkShiftDto>();
        }
    }
}
