using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Enums;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog
{
    public class StartPauseDto
    {
        public int PauseType { get; set; }
        public int WorkLogId { get; set; }
    }
}
