using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class ScreenShotValidationResponse : ValidationResponse
    {
        List<ScreenShotLogDto> ScreenShotLogDtos { get; set; }
        public ScreenShotValidationResponse(bool success, string message, string token = null,List<ScreenShotLogDto >screenShotLogDto = null) : base(success, message, token)
        {
            ScreenShotLogDtos = screenShotLogDto;
        }
    }
}
