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
                var pauseTypes =  Enum.GetValues(typeof(PauseType))
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
        // Clock In Method
        [HttpPost("ClockIn")]
        public async Task<IActionResult> ClockIn([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var existingLog =await _context.WorkTrackingLogs
                                      .FirstOrDefaultAsync(w => w.UserId == userId && (w.IsWorking == true || w.IsPaused == true));
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

            await _context.WorkTrackingLogs.AddAsync(workLog);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

            // Return WorkLogValidationResponse
            var response = new WorkLogValidationResponse(true, "Clock in successful.", result);
            return Ok(response);
        }

        // Clock Out Method
        [HttpPost("ClockOut")]
        public async Task<IActionResult> ClockOut([FromBody] int workLogId)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work log ID.");
            }

            var workLog = await _context.WorkTrackingLogs.FirstOrDefaultAsync(w => w.Id == workLogId);
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
            await _context.SaveChangesAsync();
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

            // Return WorkLogValidationResponse
            var response = new WorkLogValidationResponse(true, "Clock out successful.", result);
            return Ok(response);
        }
        // Start Pause Method with Switch Case for Pause Types
        [HttpPost("StartPause")]
        public async Task<IActionResult> StartPause([FromBody] StartPauseDto startPauseDto)
        {
            if (startPauseDto.WorkLogId <= 0)
            {
                return BadRequest("Invalid work logID.");
            }

            var workLog = await _context.WorkTrackingLogs.FirstOrDefaultAsync(w => w.Id == startPauseDto.WorkLogId);
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
                case (int)PauseType.Bathroom:
                    pausedLog.PauseType = PauseType.Bathroom;
                    break;
                case (int)PauseType.Other:
                    pausedLog.PauseType = PauseType.Other;
                    break;
                default:
                    return BadRequest("Invalid pause type.");
            }
            await _context.PauseTrackingLogs.AddAsync(pausedLog);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<PauseTrackingLogDTO>(pausedLog);

            // Return PauseLogValidationResponse
            var response = new PauseLogValidationResponse(true, "Pause started successfully.", result);
            return Ok(response);
        }

        // End Pause Method with Resuming Work Hours Logic
        [HttpPost("EndPause")]
        public async Task<IActionResult> EndPause([FromBody] int workLogId)
        {
            if (workLogId <= 0)
            {
                return BadRequest("Invalid work logID.");
            }

            var pauseTracking = await _context.PauseTrackingLogs
                .Include(p => p.WorkTrackingLog)
                .FirstOrDefaultAsync(p => p.WorkTrackingLogId == workLogId);
            if (pauseTracking == null)
            {
                return NotFound("Pause records not found.");
            }

            pauseTracking.PauseEnd = DateTime.Now;
            pauseTracking.PauseDurationInMinutes = (pauseTracking.PauseEnd - pauseTracking.PauseStart).TotalMinutes;
            pauseTracking.PauseIsFinished = true;
            pauseTracking.WorkTrackingLog.IsPaused = false;
            pauseTracking.WorkTrackingLog.IsWorking = true;

            await _context.SaveChangesAsync();

            var result = _mapper.Map<PauseTrackingLogDTO>(pauseTracking);

            // Return PauseLogValidationResponse
            var response = new PauseLogValidationResponse(true, "Pause ended and work resumed successfully.", result);
            return Ok(response);
        }
        // Get Finished Work Logs By Date
        // Get Finished Work Logs By Date 
        [HttpGet("GetFinishedWorkLogs")]
        public async Task<IActionResult> GetFinishedWorkLogs(DateTime? date)
        {
            try
            {
                // Ensure date is provided, otherwise return bad request
                if (date == null)
                {
                    return BadRequest("Date is required.");
                }

                // Access the DateTime value after checking if date is not null
                var dateValue = date.Value;

                // Construct the end-of-day DateTime for the provided date
                var endDateTime = dateValue.Date.Add(dateValue.TimeOfDay);

                // Normalize the date to ensure correct filtering (we are only interested in the date part)
                var startOfDay = dateValue.Date;

                // Filter the work logs by the specified date and time
                var finishedWorkLogs = await _context.WorkTrackingLogs
                    .Where(w => w.HasFinished == true
                                && w.WorkTimeStart >= startOfDay
                                && w.WorkTimeEnd != default
                                && w.WorkTimeEnd <= endDateTime)
                    .ToListAsync();

                // Construct the response
                var response = new
                {
                    HasWorkLogs = finishedWorkLogs.Any(),
                    Message = finishedWorkLogs.Any() ? "Finished work logs fetched successfully." : "No finished work logs found.",
                    Count = finishedWorkLogs.Count(),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work logs: {ex.Message}");
            }
        }

        // Get Paused Work Logs By Date
        [HttpGet("GetPausedWorkLogs")]
        public async Task<IActionResult> GetPausedWorkLogs()
        {
            try
            {
                // Query the WorkTrackingLogs where the work is paused
                var pausedWorkLogs = await _context.WorkTrackingLogs
                                       .Where(w => w.IsPaused == true && w.WorkTimeEnd == default)
                                       .ToListAsync();

                // Construct the response
                var response = new
                {
                    HasWorkLogs = pausedWorkLogs.Any(),
                    Message = pausedWorkLogs.Any() ? "Paused work logs fetched successfully." : "No paused work logs found.",
                    Count = pausedWorkLogs.Count()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work logs: {ex.Message}");
            }
        }
        // Get Active Work Logs By Date
        [HttpGet("GetActiveWorkLogs")]
        public async Task<IActionResult> GetActiveWorkLogs()
        {
            try
            {
                // Query the WorkTrackingLogs where the work is active
                var activeWorkLogs = await _context.WorkTrackingLogs
                                       .Where(w => w.IsWorking == true && w.WorkTimeEnd == default)
                                       .ToListAsync();

                // Construct the response
                var response = new
                {
                    HasWorkLogs = activeWorkLogs.Any(),
                    Message = activeWorkLogs.Any() ? "Active work logs fetched successfully." : "No active work logs found.",
                    Count = activeWorkLogs.Count
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work logs: {ex.Message}");
            }
        }
        // Get Work Logs by UserId and Date
        [HttpGet("GetWorkLogsByDate")]
        public async Task<IActionResult> GetWorkLogsByDate([FromQuery] string userId, [FromQuery] DateTime date)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID cannot be null or empty.");
                }

                // Normalize the date to ensure correct filtering (we are only interested in the date part)
                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                // Query the WorkTrackingLogs based on the selected date and user
                var workLog = await _context.WorkTrackingLogs
                                       .Where(w => w.UserId == userId && w.WorkTimeStart >= startOfDay && w.WorkTimeEnd <= endOfDay)
                                       .Include(w => w.PauseTrackingLogs).Include(w => w.User)
                                       .FirstOrDefaultAsync();

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

    }
}
