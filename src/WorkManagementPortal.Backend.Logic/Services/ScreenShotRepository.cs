using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class ScreenShotRepository : GenericRepository<ScreenShotTrackingLog, int>, IScreenShotRepository
    {
        private readonly ApplicationDbContext _context;
        public ScreenShotRepository(IPaginationHelper<ScreenShotTrackingLog> paginationHelper, ApplicationDbContext context) : base(paginationHelper, context)
        {
            _context = context;
        }

        public async Task UploadScreenShotAsync(FileUploadDto fileUploadDto)
        {
            byte[] screenshotBytes;
            using (var memoryStream = new MemoryStream())
            {
                await fileUploadDto.File.CopyToAsync(memoryStream);
                screenshotBytes = memoryStream.ToArray();
            }

            var screenshotTrackingLog = new ScreenShotTrackingLog
            {
                ScreenShotTime = DateTime.Now,
                UserId = fileUploadDto.UserId,
                WorkTrackingLogId = fileUploadDto.WorkLogId,
                SerializedTrackingObject = fileUploadDto.SerializedTrackingObject,
                IsIdle = fileUploadDto.IsIdle,
                Screenshot = screenshotBytes
            };

            _context.ScreenShotTrackingLogs.Add(screenshotTrackingLog);
            await _context.SaveChangesAsync();

        }

        public async Task<List<UserScreenShotLogDto>> GetScreenshotsForAllUsersAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var screenshots = await _context.ScreenShotTrackingLogs
                .Include(u => u.User)
                .ThenInclude(w => w.WorkShift).ThenInclude(w => w.WorkShiftDetails)
                .Where(s => s.ScreenShotTime >= startOfDay && s.ScreenShotTime <= endOfDay)
                .ToListAsync();
            if (screenshots.Count == 0) return new List<UserScreenShotLogDto>();

            return screenshots.GroupBy(s => s.UserId)
                .Select(g => new UserScreenShotLogDto
                {
                    UserId = g.Key,
                    UserName = g.First().User.UserName,
                    WorkShift = g.First().User.WorkShift != null
                        ? new ListWorkShiftDto
                        {
                            ShiftName = g.First().User.WorkShift.ShiftName,
                            ShiftType = g.First().User.WorkShift.ShiftType,
                            IsComplex = g.First().User.WorkShift.IsComplex,
                        }
                        : new ListWorkShiftDto { },
                    Screenshots = g.Select(s =>
                    {
                        var trackingData = JsonConvert.DeserializeObject<MouseKeyBoardTrackerDto>(s.SerializedTrackingObject);
                        return new ScreenShotLogDto
                        {
                            Id = s.Id,
                            IsIdle = s.IsIdle,
                            MouseClicks = trackingData.MouseClicks,
                            KeyBoardClicks = trackingData.KeyPresses,
                            KeyBoardInputs = trackingData.KeyInputs,
                            ScreenShotTime = s.ScreenShotTime,
                            ScreenshotFile = new FileContentResult(s.Screenshot, "image/png")
                            {
                                FileDownloadName = $"screenshot_{s.Id}.png"
                            }
                        };
                    }).ToList()
                }).ToList();
        }

        public async Task<List<UserScreenShotLogDto>> GetScreenshotsForUserAsync(string userId, DateTime date)
        {
            if (string.IsNullOrEmpty(userId)) 
                throw new ArgumentException("User ID is required.");

            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var screenshots = await _context.ScreenShotTrackingLogs
                .Include(u => u.User)
                .ThenInclude(w => w.WorkShift).ThenInclude(w=>w.WorkShiftDetails)
                .Where(s => s.UserId == userId && s.ScreenShotTime >= startOfDay && s.ScreenShotTime <= endOfDay)
                .ToListAsync();

            if (screenshots.Count == 0) return new List<UserScreenShotLogDto>();

            return screenshots.GroupBy(s => s.UserId)
                .Select(g => new UserScreenShotLogDto
                {
                    UserId = g.Key,
                    UserName = g.First().User.UserName,
                    WorkShift = g.First().User.WorkShift != null
                        ? new ListWorkShiftDto
                        {
                            ShiftName = g.First().User.WorkShift.ShiftName,
                            ShiftType = g.First().User.WorkShift.ShiftType,
                            IsComplex = g.First().User.WorkShift.IsComplex,
                        }
                        : new ListWorkShiftDto { },
                    Screenshots = g.Select(s =>
                    {
                        var trackingData = JsonConvert.DeserializeObject<MouseKeyBoardTrackerDto>(s.SerializedTrackingObject);
                        return new ScreenShotLogDto
                        {
                            Id = s.Id,
                            IsIdle = s.IsIdle,
                            MouseClicks = trackingData.MouseClicks,
                            KeyBoardClicks = trackingData.KeyPresses,
                            KeyBoardInputs = trackingData.KeyInputs,
                            ScreenShotTime = s.ScreenShotTime,
                            ScreenshotFile = new FileContentResult(s.Screenshot, "image/png")
                            {
                                FileDownloadName = $"screenshot_{s.Id}.png"
                            }
                        };
                    }).ToList()
                }).ToList();
        }
    }
}
