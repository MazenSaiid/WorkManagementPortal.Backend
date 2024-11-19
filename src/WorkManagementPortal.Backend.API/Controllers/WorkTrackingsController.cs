using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using System.Linq;
using WorkManagementPortal.Backend.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTrackingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkTrackingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Clock In Method
        [HttpPost("ClockIn")]
        public IActionResult ClockIn([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var existingLog = _context.WorkTrackingLogs
                                      .FirstOrDefault(w => w.UserId == userId && w.IsWorking == true);
            if (existingLog != null)
            {
                return BadRequest("User is already clocked in.");
            }

            var workLog = new WorkTrackingLog
            {
                UserId = userId,
                ClockIn = DateTime.Now,
                IsWorking = true,
                IsFinished =false,
                IsPaused = false
            };

            _context.WorkTrackingLogs.Add(workLog);
            _context.SaveChanges();

            return Ok(workLog);
        }

        // Clock Out Method
        [HttpPost("ClockOut")]
        public IActionResult ClockOut([FromQuery] int workLogId)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work log ID.");
            }

            var workLog = _context.WorkTrackingLogs.FirstOrDefault(w => w.Id == workLogId);
            if (workLog == null)
            {
                return NotFound("Work log not found.");
            }

            workLog.ClockOut = DateTime.UtcNow;
            workLog.IsFinished = true;
            workLog.IsWorking = false;
            workLog.IsPaused = false;

            // Calculate total worked hours excluding pauses
            var totalWorkedHours = (workLog.ClockOut - workLog.ClockIn).TotalHours;

            // Get pauses for the work log
            var pauses = _context.PauseTrackingLogs.Where(p => p.WorkLogId == workLog.Id).ToList();
            var totalPausedHours = pauses.Sum(p => p.PauseDuration) / 60;
            workLog.ActualWorkDuration = totalWorkedHours - totalPausedHours;

            return Ok(workLog);
        }

        // Start Pause Method with Switch Case for Pause Types
        [HttpPost("StartPause")]
        public IActionResult StartPause([FromQuery] int workLogId, [FromQuery] int pauseType)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work log ID.");
            }

            var workLog = _context.WorkTrackingLogs.FirstOrDefault(w => w.Id == workLogId);
            if (workLog == null)
            {
                return NotFound("Work log not found or user has already clocked out.");
            }

            workLog.IsPaused = true;
            workLog.IsWorking = false;

            var pausedLog = new PauseTrackingLog
            {
                UserId = workLog.UserId,
                WorkLogId = workLogId,
                PauseStart = DateTime.UtcNow
            };
            // Handle Pause Logic
            switch (pauseType)
            {
                case (int)PauseType.Meeting:
                    pausedLog.PauseType = PauseType.Meeting; 
                    break;
                case (int)PauseType.Break:
                    pausedLog.PauseType = PauseType.Meeting;
                    break;
                default:
                    return BadRequest("Invalid pause type.");
            }
            _context.PauseTrackingLogs.Add(pausedLog);
            _context.SaveChanges();
            return Ok(workLog);
        }

        // End Pause Method with Resuming Work Hours Logic
        [HttpPost("EndPause")]
        public IActionResult EndPause([FromQuery] int pauseTrackingId)
        {
            if (pauseTrackingId <= 0)
            {
                return BadRequest("Invalid pause tracking ID.");
            }

            var pauseTracking = _context.PauseTrackingLogs.
                Include(p => p.WorkTrackingLog).
                FirstOrDefault(p => p.Id == pauseTrackingId);
            if (pauseTracking == null)
            {
                return NotFound("Pause tracking not found.");
            }
            
            pauseTracking.PauseEnd = DateTime.UtcNow;
            pauseTracking.PauseDuration = (pauseTracking.PauseEnd - pauseTracking.PauseStart).TotalMinutes;

            pauseTracking.WorkTrackingLog.IsPaused = false;
            pauseTracking.WorkTrackingLog.IsWorking = true;

            _context.SaveChanges();

            return Ok(pauseTracking);
        }

        // Get Work Log by ID (for validation, debugging, etc.)
        [HttpGet("worklog/{workLogId}")]
        public IActionResult GetWorkLogById([FromRoute] int workLogId)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work log ID.");
            }

            var workLog = _context.WorkTrackingLogs.FirstOrDefault(w => w.Id == workLogId);
            if (workLog == null)
            {
                return NotFound("Work log not found.");
            }

            return Ok(workLog);
        }
    }
}
