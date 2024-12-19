using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot
{
    public class ScreenShotLogDto
    {
        public int Id { get; set; }
        public DateTime ScreenShotTime { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }  // Assuming you might want to include the user's name
        public string ScreenshotBase64 { get; set; } // Base64 encoded screenshot data
        public ListWorkShiftDto WorkShift { get; set; }  // Work shift details for the user

    }
}
