using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class WorkLogValidationResponse : ValidationResponse
    {
        public WorkTrackingLogDTO WorkTrackingLog { get; set; }
        public WorkLogValidationResponse(bool success, string message, WorkTrackingLogDTO workTrackingLog, string token = null)
            : base(success, message, token)
        {
            WorkTrackingLog = workTrackingLog;
        }
    }
}
