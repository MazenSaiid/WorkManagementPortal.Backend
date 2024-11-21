using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using System.Linq;
using WorkManagementPortal.Backend.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using AutoMapper;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTrackingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public WorkTrackingsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                                      .FirstOrDefault(w => w.UserId == userId && (w.IsWorking == true || w.IsPaused == true));
            if (existingLog != null)
            {
                return BadRequest("User is already already working.");
            }

            var workLog = new WorkTrackingLog
            {
                UserId = userId,
                WorkTimeStart = DateTime.Now,
                WorkDate = DateOnly.FromDateTime(DateTime.Now),
                IsWorking = true,
                IsPaused = false
            };

            _context.WorkTrackingLogs.Add(workLog);
            _context.SaveChanges();
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);
            return Ok(result);
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

            workLog.WorkTimeEnd = DateTime.Now;
            workLog.IsWorking = false;
            workLog.IsPaused = false;
            workLog.HasFinished = true;

            // Calculate total worked hours excluding pauses
            var totalWorkedHours = (workLog.WorkTimeEnd - workLog.WorkTimeStart).TotalHours;

            // Get pauses for the work log
            var pauses = _context.PauseTrackingLogs.Where(p => p.WorkTrackingLogId == workLog.Id).ToList();
            var totalPausedHours = pauses.Sum(p => p.PauseDurationInMinutes) / 60;
            workLog.ActualWorkDurationInHours = totalWorkedHours - totalPausedHours;
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);
            return Ok(result);
        }

        // Start Pause Method with Switch Case for Pause Types
        [HttpPost("StartPause")]
        public IActionResult StartPause([FromQuery] int workLogId, [FromQuery] int pauseType)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work logID.");
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
                WorkTrackingLogId = workLogId,
                WorkDate = DateOnly.FromDateTime(DateTime.Now),
                PauseStart = DateTime.Now
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
                case (int)PauseType.InCall:
                    pausedLog.PauseType = PauseType.InCall;
                    break;
                default:
                    return BadRequest("Invalid pause type.");
            }
            _context.PauseTrackingLogs.Add(pausedLog);
            _context.SaveChanges();

            var result = _mapper.Map<PauseTrackingLogDTO>(pausedLog);
            return Ok(result);
        }

        // End Pause Method with Resuming Work Hours Logic
        [HttpPost("EndPause")]
        public IActionResult EndPause([FromQuery] int workLogId)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work logID.");
            }

            var pauseTracking = _context.PauseTrackingLogs.
                Include(p => p.WorkTrackingLog).
                FirstOrDefault(p => p.WorkTrackingLogId == workLogId);
            if (pauseTracking == null)
            {
                return NotFound("Pause records not found.");
            }
            
            pauseTracking.PauseEnd = DateTime.Now;
            pauseTracking.PauseDurationInMinutes = (pauseTracking.PauseEnd - pauseTracking.PauseStart).TotalMinutes;

            pauseTracking.WorkTrackingLog.IsPaused = false;
            pauseTracking.WorkTrackingLog.IsWorking = true;

            _context.SaveChanges();

            var result = _mapper.Map<PauseTrackingLogDTO>(pauseTracking);
            return Ok(result);
        }

        // Get Work Log by ID (for validation, debugging, etc.)
        [HttpGet("WorkLog/{workLogId}")]
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
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);
            return Ok(result);
        }
    }
}
