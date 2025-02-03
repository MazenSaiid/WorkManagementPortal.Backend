using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IScreenShotRepository : IGenericRepository<ScreenShotTrackingLog, int>
    {
        Task UploadScreenShotAsync(FileUploadDto fileUploadDto);
        Task<List<UserScreenShotLogDto>> GetScreenshotsForAllUsersAsync(DateTime date);
        Task<List<UserScreenShotLogDto>> GetScreenshotsForUserAsync(string userId, DateTime date);
    }
}
