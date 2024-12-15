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
using WorkManagementPortal.Backend.Logic.Responses;

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
        [HttpGet]
        [Route("GetPauseTypes")]
        public IActionResult GetPauseTypes()
        {
            try
            {
                // Get enum values from PauseType enum
                var pauseTypes = Enum.GetValues(typeof(PauseType))
                                     .Cast<PauseType>()
                                     .Select(e => new { id = (int)e, name = e.ToString() })
                                     .ToList();

                return Ok(pauseTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching pause types: {ex.Message}");
            }
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
        // Clock In Method
        [HttpPost("ClockIn")]
        public IActionResult ClockIn([FromBody] string userId)
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

            // Return WorkLogValidationResponse
            var response = new WorkLogValidationResponse(true, "Clock in successful.", result);
            return Ok(response);
        }

        // Clock Out Method
        [HttpPost("ClockOut")]
        public IActionResult ClockOut([FromBody] int workLogId)
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
            foreach (var pause in pauses)
            {
                if (!pause.PauseIsFinished)
                {
                    pause.PauseEnd = DateTime.Now;
                    pause.PauseDurationInMinutes = (pause.PauseEnd - pause.PauseStart).TotalMinutes;
                    pause.PauseIsFinished = true;  // Mark the pause as finished
                }
            }

            var totalPausedHours = pauses.Sum(p => p.PauseDurationInMinutes) / 60;
            workLog.ActualWorkDurationInHours = totalWorkedHours - totalPausedHours;
            _context.SaveChanges();
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

            // Return WorkLogValidationResponse
            var response = new WorkLogValidationResponse(true, "Clock out successful.", result);
            return Ok(response);
        }

        // Get Work Logs by UserId and Date
        [HttpGet("GetWorkLogsByDate")]
        public IActionResult GetWorkLogsByDate([FromQuery] string userId, [FromQuery] DateTime date)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID cannot be null or empty.");
                }

                // Query the WorkTrackingLogs based on the selected date and user
                var workLog = _context.WorkTrackingLogs
                                       .Where(w => w.UserId == userId && w.WorkDate == DateOnly.FromDateTime(date))
                                       .Include(w => w.PauseTrackingLogs).Include(w => w.User)
                                       .FirstOrDefault();

                if (workLog == null )
                {
                    return NotFound("No work logs found for the given date.");
                }

                var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

                // Return WorkLogValidationResponse
                var response = new WorkLogValidationResponse(true, "Work log fetched successfully.", result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work logs: {ex.Message}");
            }
        }

        // Start Pause Method with Switch Case for Pause Types
        [HttpPost("StartPause")]
        public IActionResult StartPause([FromBody] StartPauseDto startPauseDto)
        {
            if (startPauseDto.WorkLogId <= 0)
            {
                return BadRequest("Invalid work logID.");
            }

            var workLog = _context.WorkTrackingLogs.FirstOrDefault(w => w.Id == startPauseDto.WorkLogId);
            if (workLog == null)
            {
                return NotFound("Work log not found or user has already clocked out.");
            }

            workLog.IsPaused = true;
            workLog.IsWorking = false;

            var pausedLog = new PauseTrackingLog
            {
                UserId = workLog.UserId,
                WorkTrackingLogId = startPauseDto.WorkLogId,
                WorkDate = DateOnly.FromDateTime(DateTime.Now),
                PauseStart = DateTime.Now
            };
            // Handle Pause Logic
            switch (startPauseDto.PauseType)
            {
                case (int)PauseType.Meeting:
                    pausedLog.PauseType = PauseType.Meeting;
                    break;
                case (int)PauseType.Break:
                    pausedLog.PauseType = PauseType.Break;
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

            // Return PauseLogValidationResponse
            var response = new PauseLogValidationResponse(true, "Pause started successfully.", result);
            return Ok(response);
        }

        // End Pause Method with Resuming Work Hours Logic
        [HttpPost("EndPause")]
        public IActionResult EndPause([FromBody] int workLogId)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work logID.");
            }

            var pauseTracking = _context.PauseTrackingLogs
                .Include(p => p.WorkTrackingLog)
                .FirstOrDefault(p => p.WorkTrackingLogId == workLogId);
            if (pauseTracking == null)
            {
                return NotFound("Pause records not found.");
            }

            pauseTracking.PauseEnd = DateTime.Now;
            pauseTracking.PauseDurationInMinutes = (pauseTracking.PauseEnd - pauseTracking.PauseStart).TotalMinutes;
            pauseTracking.PauseIsFinished = true;
            pauseTracking.WorkTrackingLog.IsPaused = false;
            pauseTracking.WorkTrackingLog.IsWorking = true;

            _context.SaveChanges();

            var result = _mapper.Map<PauseTrackingLogDTO>(pauseTracking);

            // Return PauseLogValidationResponse
            var response = new PauseLogValidationResponse(true, "Pause ended and work resumed successfully.", result);
            return Ok(response);
        }


    }
}
