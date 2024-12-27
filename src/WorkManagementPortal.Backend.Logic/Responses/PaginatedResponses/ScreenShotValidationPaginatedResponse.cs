using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class ScreenShotValidationPaginatedResponse : ValidationResponse
    {
        public IEnumerable<UserScreenShotLogDto> UserScreenShotLogDtos { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public ScreenShotValidationPaginatedResponse(bool success, string message, int currentPage = 1, int pageSize = 20, int totalCount = 0,  string token = null, IEnumerable<UserScreenShotLogDto> userScreenShotLogDto = null) : base(success, message, token)
        {
            UserScreenShotLogDtos = userScreenShotLogDto ?? new List<UserScreenShotLogDto>();
            // Set pagination properties
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
