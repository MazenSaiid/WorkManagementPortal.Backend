using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class ScreenShotValidationResponse : ValidationResponse
    {
        public IEnumerable<UserScreenShotLogDto> UserScreenShotLogDtos { get; set; }
        public ScreenShotValidationResponse(bool success, string message, string token = null, IEnumerable<UserScreenShotLogDto> userScreenShotLogDto = null) : base(success, message, token)
        {
            UserScreenShotLogDtos = userScreenShotLogDto ?? new List<UserScreenShotLogDto>(); ;
        }
    }
}
