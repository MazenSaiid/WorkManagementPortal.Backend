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
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTrackingsController : ControllerBase
    {
        private readonly IWorkTrackingRepository _workTrackingRepository;
        private readonly IMapper _mapper;

        public WorkTrackingsController(IWorkTrackingRepository workTrackingRepository, IMapper mapper)
        {
            _workTrackingRepository = workTrackingRepository;
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
        // Clock In Method
        [HttpPost("ClockIn")]
        public async Task<IActionResult> ClockIn([FromBody] string userId)
        {
            try
            {
                var response = await _workTrackingRepository.ClockInAsync(userId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred : {ex.Message}, couldn't Clock in for User {userId}");
            }
        }

        // Clock Out Method
        [HttpPost("ClockOut")]
        public async Task<IActionResult> ClockOut([FromBody] int workLogId)
        {
            try
            {
                if (workLogId <= 0)
                {
                    return BadRequest("Invalid work log ID.");
                }
                var response = await _workTrackingRepository.ClockOutAsync(workLogId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message},couldn't Clock Out for WorkLog {workLogId}");
            }

        }
        // Start Pause Method with Switch Case for Pause Types
        [HttpPost("StartPause")]
        public async Task<IActionResult> StartPause([FromBody] StartPauseDto startPauseDto)
        {

            try
            {
                if (startPauseDto.WorkLogId <= 0)
                {
                    return BadRequest("Invalid work logID.");
                }
                var response = await _workTrackingRepository.StartPauseAsync(startPauseDto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message},couldn't Start Pause for WorkLog {startPauseDto.WorkLogId}");
            }

        }

        // End Pause Method with Resuming Work Hours Logic
        [HttpPost("EndPause")]
        public async Task<IActionResult> EndPause([FromBody] int workLogId)
        {

            try
            {
                if (workLogId <= 0)
                {
                    return BadRequest("Invalid work logID.");
                }
                var response = await _workTrackingRepository.EndPauseAsync(workLogId);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message},couldn't End Pause for WorkLog {workLogId}");
            }
        }
        // Get Finished Work Logs By Date
        [HttpGet("GetFinishedWorkLogs")]
        public async Task<IActionResult> GetFinishedWorkLogs(DateTime? date)
        {
            try
            {
                // Ensure date is provided, otherwise return bad request
                if (date == null && !date.HasValue)
                {
                    return BadRequest("Date is required.");
                }

                var response = await _workTrackingRepository.GetFinishedWorkLogsAsync(date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching finished work logs: {ex.Message}");
            }
        }
        // Get Paused Work Logs By Date
        [HttpGet("GetPausedWorkLogs")]
        public async Task<IActionResult> GetPausedWorkLogs(DateTime? date)
        {
            try
            {
                // Ensure date is provided, otherwise return bad request
                if (date == null && !date.HasValue)
                {
                    return BadRequest("Date is required.");
                }
                var response = await _workTrackingRepository.GetPausedWorkLogsAsync( date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching paused work logs: {ex.Message}");
            }
        }
        // Get Active Work Logs By Date
        [HttpGet("GetActiveWorkLogs")]
        public async Task<IActionResult> GetActiveWorkLogs(DateTime? date)
        {
            try
            {
                // Ensure date is provided, otherwise return bad request
                if (date == null && !date.HasValue)
                {
                    return BadRequest("Date is required.");
                }
                var response = await _workTrackingRepository.GetActiveWorkLogsAsync(date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching active work logs: {ex.Message}");
            }
        }
        [HttpGet("GetLateCheckInWorkLogs")]
        public async Task<IActionResult> GetLateCheckInWorkLogs(DateTime? date)
        {
            try
            {
                // Ensure date is provided, otherwise return bad request
                if (date == null && !date.HasValue)
                {
                    return BadRequest("Date is required.");
                }
                var response = await _workTrackingRepository.GetLateCheckInWorkLogsAsync(date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching Late CheckIn work logs: {ex.Message}");
            }
        }
        [HttpGet("GetEarlyCheckoutWorkLogs")]
        public async Task<IActionResult> GetEarlyCheckoutWorkLogs(DateTime? date)
        {
            try
            {
                // Ensure date is provided, otherwise return bad request
                if (date == null && !date.HasValue)
                {
                    return BadRequest("Date is required.");
                }
                var response = await _workTrackingRepository.GetEarlyCheckoutWorkLogsAsync(date);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching Early Checkout work logs: {ex.Message}");
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
                // Ensure date is provided, otherwise return bad request
                if (date == null)
                {
                    return BadRequest("Date is required.");
                }
                var response = await _workTrackingRepository.GetWorkLogsByDateAsync(userId, date);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching work logs: {ex.Message}");
            }
        }

    } 
}

