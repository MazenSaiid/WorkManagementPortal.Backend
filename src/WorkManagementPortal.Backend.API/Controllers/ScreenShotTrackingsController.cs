using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;
using Microsoft.IdentityModel.Tokens;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenShotTrackingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScreenShotTrackingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("UploadScreenShot")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadScreenShot([FromForm] FileUploadDto fileUploadDto)
        {
            try
            {
                if (fileUploadDto.File == null || fileUploadDto.File.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                if (string.IsNullOrEmpty(fileUploadDto.UserId))
                {
                    return BadRequest("User ID is required.");
                }

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
                    Screenshot = screenshotBytes
                };

                _context.ScreenShotTrackingLogs.Add(screenshotTrackingLog);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Screenshot uploaded successfully.", ScreenshotId = screenshotTrackingLog.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while uploading the screenshot.", Exception = ex.Message });
            }
        }

        // Method to get all screenshots for a user on a specific date
        [HttpGet("GetScreenshotsForAllUsers")]
        public async Task<IActionResult> GetScreenshotsForAllUsers([FromQuery] DateTime date )
        {
            try
            {
                // Normalize the date to ensure correct filtering (we are only interested in the date part)
                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                // Query the database to find screenshots taken on the given date
                var screenshots = await _context.ScreenShotTrackingLogs
                    .Include(u => u.User)
                    .ThenInclude(w => w.WorkShift)
                    .Where(s => s.ScreenShotTime >= startOfDay && s.ScreenShotTime <= endOfDay)
                    .ToListAsync();

                if (screenshots == null || screenshots.Count == 0)
                {
                    // Return validation response with no screenshots found
                    return Ok(new ScreenShotValidationResponse(false, "No screenshots found for the specified date."));
                }

                // Group the screenshots by UserId
                var groupedScreenshots = screenshots
                    .GroupBy(s => s.UserId)
                    .Select(g => new UserScreenShotLogDto
                    {

                        UserId = g.Key,
                        UserName = g.First().User.UserName, 
                        WorkShift = g.First().User.WorkShift != null ? new ListWorkShiftDto
                        {
                            ShiftName = g.First().User.WorkShift.ShiftName,
                            ShiftType = g.First().User.WorkShift.ShiftType,
                            StartTime = g.First().User.WorkShift.StartTime,
                            EndTime = g.First().User.WorkShift.EndTime
                        } : new ListWorkShiftDto { }
                        ,
                        Screenshots = g.Select(s => new ScreenShotLogDto
                        {
                            Id = s.Id,
                            ScreenShotTime = s.ScreenShotTime,
                            ScreenshotFile = new FileContentResult(s.Screenshot, "image/png")
                            {
                                FileDownloadName = $"screenshot_{s.Id}.png"
                            }
                        }).ToList()
                    }).ToList();

                // Return the grouped screenshots data
                //return Ok(groupedScreenshots);
                return Ok(new ScreenShotValidationResponse(true, "Screenshots retrieved successfully.", null, groupedScreenshots));
            }
            catch (Exception ex)
            {
                // Return error response in case of an exception
                return StatusCode(500, new ScreenShotValidationResponse(false, "An error occurred while retrieving the screenshots.", ex.Message));
            }
        }



        // Method to get all screenshots for a user on a specific date
        [HttpGet("GetScreenshotsForUser")]
        public async Task<IActionResult> GetScreenshotsForUser([FromQuery] string userId, [FromQuery] DateTime date )
        {
            try
            {
                // Ensure userId is not null or empty
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID is required.");
                }

                // Normalize the date to ensure correct filtering (we are only interested in the date part)
                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                // Query the database to find screenshots taken on the given date by the user
                var screenshots = await _context.ScreenShotTrackingLogs
                    .Include(u => u.User)
                    .ThenInclude(w => w.WorkShift)
                    .Where(s => s.UserId == userId && s.ScreenShotTime >= startOfDay && s.ScreenShotTime <= endOfDay)
                    .ToListAsync();

                if (screenshots == null || screenshots.Count == 0)
                {
                    // Return validation response with no screenshots
                    return Ok(new ScreenShotValidationResponse(false, "No screenshots found for this user on the specified date."));
                }

                var screenshotData =  screenshots
                    .GroupBy(s => s.UserId)
                    .Select(g => new UserScreenShotLogDto
                    {

                        UserId = g.Key,
                        UserName = g.First().User.UserName,
                        WorkShift = g.First().User.WorkShift != null ? new ListWorkShiftDto
                        {
                            ShiftName = g.First().User.WorkShift.ShiftName,
                            ShiftType = g.First().User.WorkShift.ShiftType,
                            StartTime = g.First().User.WorkShift.StartTime,
                            EndTime = g.First().User.WorkShift.EndTime
                        } : new ListWorkShiftDto { }
                        ,
                        Screenshots = g.Select(s => new ScreenShotLogDto
                        {
                            Id = s.Id,
                            ScreenShotTime = s.ScreenShotTime,
                            ScreenshotFile = new FileContentResult(s.Screenshot, "image/png")
                            {
                                FileDownloadName = $"screenshot_{s.Id}.png"
                            }
                        }).ToList()
                    }).ToList();

                // Return the grouped screenshots data
                return Ok(new ScreenShotValidationResponse(true, "Screenshots retrieved successfully.", null, screenshotData));

            }
            catch (Exception ex)
            {
                // Return error response in case of an exception
                return StatusCode(500, new ScreenShotValidationResponse(false, "An error occurred while retrieving the screenshots.", ex.Message));
            }
        }

    }
}
