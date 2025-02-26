using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot
{
    public class FileUploadDto
    {
        public IFormFile File { get; set; } 
        public string UserId { get; set; }
        public int? WorkLogId { get; set; }
        public bool IsIdle { get; set;}
        public string SerializedTrackingObject { get; set; }

    }
}
