using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot
{
    public class UserScreenShotLogDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }  // Assuming you might want to include the user's name
        public ListWorkShiftDto WorkShift { get; set; }  // Work shift details for the user
        public List<ScreenShotLogDto> Screenshots { get; set; }

    }
    public class ScreenShotLogDto
    {
        public int Id { get; set; }
        public DateTime ScreenShotTime { get; set; }
        public FileContentResult ScreenshotFile { get; set; } // Base64 encoded screenshot data

    }
}
