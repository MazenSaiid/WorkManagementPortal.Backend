using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot
{
    public class MouseKeyBoardTrackerDto
    {
        public int MouseClicks { get; set; }
        public int KeyPresses { get; set; }
        public bool IsIdle { get; set; }
        public List<string> KeyInputs { get; set; }
    }
}
