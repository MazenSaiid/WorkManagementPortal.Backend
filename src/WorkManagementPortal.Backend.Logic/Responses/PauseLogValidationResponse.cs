using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class PauseLogValidationResponse : ValidationResponse
    { 
        public PauseTrackingLogDTO PauseTrackingLog { get; set; }
        public PauseLogValidationResponse(bool success, string message, PauseTrackingLogDTO pauseTrackingLog, string token = null)
            : base(success, message, token)
        {
            PauseTrackingLog = pauseTrackingLog;
        }
    }
}
