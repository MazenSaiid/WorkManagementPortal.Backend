using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;
using WorkManagementPortal.Backend.Infrastructure.Enums;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class WorkTrackingRepository : GenericRepository<WorkTrackingLog, int>, IWorkTrackingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public WorkTrackingRepository(IPaginationHelper<WorkTrackingLog> paginationHelper, ApplicationDbContext context, IMapper mapper) : base(paginationHelper, context)
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task<WorkLogValidationResponse> ClockInAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.");
            }

            var existingLog = await _context.WorkTrackingLogs
                .FirstOrDefaultAsync(w => w.UserId == userId && (w.IsWorking || w.IsPaused));

            if (existingLog != null)
            {
                throw new ArgumentException("User is already working.");
            }

            var user = await _context.Users
                .Include(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails) // Include related WorkShiftDetails
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (user.WorkShift != null && user.WorkShift.IsComplex)
            {
                
                var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                var currentDay = DateTime.Now.DayOfWeek;

                
                    // Fetch today's shift details
                    var todayShiftDetail = user.WorkShift.WorkShiftDetails
                        .FirstOrDefault(detail => detail.Day == currentDay);

                    if (todayShiftDetail == null)
                    {
                        throw new ArgumentException("No shift scheduled for today.");
                    }

                
                var shiftStartTime = todayShiftDetail.StartTime;

                // Adjust shift start time with a grace period
                var adjustedStartTime = shiftStartTime.Add(TimeSpan.FromMinutes(5));
                var startedLate = currentTime > adjustedStartTime;

                var workLog = new WorkTrackingLog
                {
                    UserId = userId,
                    WorkTimeStart = DateTime.Now,
                    WorkDate = DateOnly.FromDateTime(DateTime.Now),
                    IsWorking = true,
                    IsPaused = false,
                    HasCheckedInLate = startedLate,
                };

                await _context.WorkTrackingLogs.AddAsync(workLog);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

                // Return WorkLogValidationResponse
                var response = new WorkLogValidationResponse(true, "Clock in successful.", result);
                return response;
            }

            throw new ArgumentException("WorkShift not defined for the user.");
        }
        public async Task<WorkLogValidationResponse> ClockOutAsync(int workLogId)
        {
            if (workLogId <= 0)
            {
                throw new ArgumentException("Invalid work log ID.");
            }

            var workLog = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .FirstOrDefaultAsync(w => w.Id == workLogId);

            if (workLog == null)
            {
                throw new KeyNotFoundException("Work log not found.");
            }

            var user = workLog.User;
            if (user == null || user.WorkShift == null)
            {
                throw new ArgumentException("Work shift not associated with the user.");
            }

            bool hasCheckedOutEarly = false;
            TimeOnly? shiftEndTime = null;
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var currentDay = DateTime.Now.DayOfWeek;

            if (user.WorkShift != null && user.WorkShift.IsComplex)
            {
                // Fetch today's shift details
                var todayShiftDetail = user.WorkShift.WorkShiftDetails
                    .FirstOrDefault(detail => detail.Day == currentDay);

                if (todayShiftDetail == null)
                {
                    throw new ArgumentException("No shift scheduled for today.");
                }

                shiftEndTime = todayShiftDetail.EndTime;

            }

            // Adjust shift end time with an early checkout grace period
            var adjustedEndTime = shiftEndTime.Value.Add(TimeSpan.FromMinutes(-5));

            // Check if the shift end time is before the current time
            if (currentTime < adjustedEndTime)
            {
                hasCheckedOutEarly = true;
            }

            workLog.WorkTimeEnd = DateTime.Now;
            workLog.IsWorking = false;
            workLog.IsPaused = false;
            workLog.HasFinished = true;
            workLog.HasCheckedOutAEarly = hasCheckedOutEarly;

            // Calculate total worked hours excluding pauses
            var totalWorkedHours = (workLog.WorkTimeEnd - workLog.WorkTimeStart).TotalHours;

            // Get pauses for the work log
            var pauses = await _context.PauseTrackingLogs
                .Where(p => p.WorkTrackingLogId == workLog.Id)
                .ToListAsync();

            foreach (var pause in pauses)
            {
                if (!pause.PauseIsFinished)
                {
                    pause.PauseEnd = DateTime.Now;
                    pause.PauseDurationInMinutes = (pause.PauseEnd - pause.PauseStart).TotalMinutes;
                    pause.PauseIsFinished = true; // Mark the pause as finished
                }
            }

            var totalPausedHours = pauses.Sum(p => p.PauseDurationInMinutes) / 60;
            workLog.ActualWorkDurationInHours = totalWorkedHours - totalPausedHours;

            await _context.SaveChangesAsync();
            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

            // Return WorkLogValidationResponse
            var response = new WorkLogValidationResponse(true, "Clock out successful.", result);
            return response;
        }

        public async Task<PauseLogValidationResponse> StartPauseAsync(StartPauseDto startPauseDto)
        {
            if (startPauseDto.WorkLogId <= 0)
            {
                throw new ArgumentException("Invalid work log ID.");
            }

            var workLog = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .FirstOrDefaultAsync(w => w.Id == startPauseDto.WorkLogId);

            if (workLog == null)
            {
                throw new KeyNotFoundException("Work log not found or user has already clocked out.");
            }

            var userShift = workLog.User?.WorkShift;
            if (userShift == null)
            {
                throw new ArgumentException("No work shift is associated with this user.");
            }

            // Validate against shift details if the shift is complex
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var currentDay = DateTime.Now.DayOfWeek;
            if (userShift.IsComplex)
            {
                var todayShift = userShift.WorkShiftDetails.FirstOrDefault(d => d.Day == currentDay);
                if (todayShift == null || currentTime < todayShift.StartTime || currentTime > todayShift.EndTime)
                {
                    throw new InvalidOperationException("Cannot start a pause outside of shift hours.");
                }
            }

            workLog.IsPaused = true;
            workLog.IsWorking = false;

            var pausedLog = new PauseTrackingLog
            {
                UserId = workLog.UserId,
                WorkTrackingLogId = startPauseDto.WorkLogId,
                WorkDate = DateOnly.FromDateTime(DateTime.Now),
                PauseStart = DateTime.Now,
                PauseType = startPauseDto.PauseType switch
                {
                    (int)PauseType.Meeting => PauseType.Meeting,
                    (int)PauseType.Break => PauseType.Break,
                    (int)PauseType.InCall => PauseType.InCall,
                    (int)PauseType.Bathroom => PauseType.Bathroom,
                    (int)PauseType.Other => PauseType.Other,
                    _ => throw new ArgumentException("Invalid pause type.")
                }
            };

            await _context.PauseTrackingLogs.AddAsync(pausedLog);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<PauseTrackingLogDTO>(pausedLog);

            // Return PauseLogValidationResponse
            var response = new PauseLogValidationResponse(true, "Pause started successfully.", result);
            return response;
        }
        public async Task<PauseLogValidationResponse> EndPauseAsync(int workLogId)
        {
            if (workLogId <= 0)
            {
                throw new ArgumentException("Invalid work log ID.");
            }

            var pauseTracking = await _context.PauseTrackingLogs
                .Include(p => p.WorkTrackingLog)
                .ThenInclude(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .FirstOrDefaultAsync(p => p.WorkTrackingLogId == workLogId);

            if (pauseTracking == null)
            {
                throw new KeyNotFoundException("Pause records not found.");
            }

            var userShift = pauseTracking.WorkTrackingLog?.User?.WorkShift;
            if (userShift == null)
            {
                throw new ArgumentException("No work shift is associated with this user.");
            }

            // Validate end time against the shift's details if the shift is complex
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var currentDay = DateTime.Now.DayOfWeek;
            if (userShift.IsComplex)
            {
                var todayShift = userShift.WorkShiftDetails.FirstOrDefault(d => d.Day == currentDay);
                if (todayShift == null || currentTime > todayShift.EndTime)
                {
                    throw new InvalidOperationException("Cannot end a pause outside of shift hours.");
                }
            }

            pauseTracking.PauseEnd = DateTime.Now;
            pauseTracking.PauseDurationInMinutes = (pauseTracking.PauseEnd - pauseTracking.PauseStart).TotalMinutes;
            pauseTracking.PauseIsFinished = true;

            var workLog = pauseTracking.WorkTrackingLog;
            workLog.IsPaused = false;
            workLog.IsWorking = true;

            await _context.SaveChangesAsync();

            var result = _mapper.Map<PauseTrackingLogDTO>(pauseTracking);

            // Return PauseLogValidationResponse
            var response = new PauseLogValidationResponse(true, "Pause ended and work resumed successfully.", result);
            return response;
        }

        public async Task<List<WorkTrackingLog>> GetFinishedWorkLogsAsync(DateTime? date)
        {
            var dateValue = date.Value;
            var endDateTime = dateValue.Date.Add(dateValue.TimeOfDay);
            var startOfDay = dateValue.Date;
            var dayOfWeek = dateValue.DayOfWeek;

            var workLogs = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .Where(w => w.HasFinished == true &&
                            w.WorkDate == DateOnly.FromDateTime(startOfDay))
                .ToListAsync();
            var finishedWorkLogs = new List<WorkTrackingLog>();

            foreach (var log in workLogs)
            {
                var workShift = log.User?.WorkShift;

                if (workShift != null && workShift.IsComplex)
                {
                    // Fetch the WorkShiftDetail for the specific day
                    var todayShift = workShift.WorkShiftDetails.FirstOrDefault(d => d.Day == dayOfWeek);
                    if (todayShift != null)
                    {
                        // Validate against the day's specific start and end time
                        if (TimeOnly.FromDateTime(log.WorkTimeStart) >= todayShift.StartTime && TimeOnly.FromDateTime(log.WorkTimeStart) <= todayShift.EndTime)
                        {
                            finishedWorkLogs.Add(log);
                        }
                    }
                }
    
            }

            return finishedWorkLogs;
        }

        public async Task<List<WorkTrackingLog>> GetPausedWorkLogsAsync(DateTime? date)
        {
            var dateValue = date.Value;
            var endDateTime = dateValue.Date.Add(dateValue.TimeOfDay);
            var startOfDay = dateValue.Date;
            var dayOfWeek = dateValue.DayOfWeek;

            // Query the WorkTrackingLogs where the work is paused
            var workLogs = await _context.WorkTrackingLogs
                                    .Include(w => w.User)
                                    .ThenInclude(u => u.WorkShift)
                                    .ThenInclude(ws => ws.WorkShiftDetails)
                                   .Where(w => w.IsPaused == true
                                   && w.WorkTimeStart >= startOfDay
                                   && w.WorkTimeEnd != default
                                   && w.WorkTimeEnd <= endDateTime)
                                   .ToListAsync();

            var pausedWorkLogs = new List<WorkTrackingLog>();

            foreach (var log in workLogs)
            {
                var workShift = log.User?.WorkShift;

                if (workShift != null && workShift.IsComplex)
                {
                    // Fetch the WorkShiftDetail for the specific day
                    var todayShift = workShift.WorkShiftDetails.FirstOrDefault(d => d.Day == dayOfWeek);
                    if (todayShift != null)
                    {
                        // Validate against the day's specific start and end time
                        if (TimeOnly.FromDateTime(log.WorkTimeStart) >= todayShift.StartTime && TimeOnly.FromDateTime(log.WorkTimeStart) <= todayShift.EndTime)
                        {
                            pausedWorkLogs.Add(log);
                        }
                    }
                }
            }
            return pausedWorkLogs;
        }

        public async Task<List<WorkTrackingLog>> GetActiveWorkLogsAsync(DateTime? date)
        {
            if (date == null)
            {
                throw new ArgumentException("Date cannot be null.");
            }

            var dateValue = date.Value;
            var startOfDay = dateValue.Date;
            var endOfDay = dateValue.Date.Add(dateValue.TimeOfDay);
            var dayOfWeek = dateValue.DayOfWeek;

            // Query WorkTrackingLogs and related data
            var workLogs = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .Where(w => w.IsWorking && w.WorkDate == DateOnly.FromDateTime(startOfDay))
                .ToListAsync();

            var activeWorkLogs = new List<WorkTrackingLog>();

            foreach (var log in workLogs)
            {
                var workShift = log.User?.WorkShift;

                if (workShift != null && workShift.IsComplex)
                {
                    // Fetch the WorkShiftDetail for the specific day
                    var todayShift = workShift.WorkShiftDetails.FirstOrDefault(d => d.Day == dayOfWeek);
                    if (todayShift != null)
                    {
                        // Validate against the day's specific start and end time
                        if (TimeOnly.FromDateTime(log.WorkTimeStart) >= todayShift.StartTime && TimeOnly.FromDateTime(log.WorkTimeStart) <= todayShift.EndTime)
                        {
                            activeWorkLogs.Add(log);
                        }
                    }
                }
            }

            return activeWorkLogs;
        }

        public async Task<List<WorkTrackingLog>> GetLateCheckInWorkLogsAsync(DateTime? date)
        {

            var dateValue = date.Value;
            var endDateTime = dateValue.Date.Add(dateValue.TimeOfDay);
            var startOfDay = dateValue.Date;
            var dayOfWeek = dateValue.DayOfWeek;

            // Query the WorkTrackingLogs where the work is late
            var workLogs = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .Where(w => w.HasCheckedInLate == true && w.WorkDate == DateOnly.FromDateTime(startOfDay))
                .ToListAsync();

            var lateChekInWorkLogs = new List<WorkTrackingLog>();

            foreach (var log in workLogs)
            {
                var workShift = log.User?.WorkShift;

                if (workShift != null && workShift.IsComplex)
                {
                    // Fetch the WorkShiftDetail for the specific day
                    var todayShift = workShift.WorkShiftDetails.FirstOrDefault(d => d.Day == dayOfWeek);
                    if (todayShift != null)
                    {
                        // Validate against the day's specific start and end time
                        if (TimeOnly.FromDateTime(log.WorkTimeStart) >= todayShift.StartTime && TimeOnly.FromDateTime(log.WorkTimeStart) <= todayShift.EndTime)
                        {
                            lateChekInWorkLogs.Add(log);
                        }
                    }
                }
                
            }

            return lateChekInWorkLogs;
        }
        public async Task<List<WorkTrackingLog>> GetEarlyCheckoutWorkLogsAsync(DateTime? date)
        {
            var dateValue = date.Value;
            var endDateTime = dateValue.Date.Add(dateValue.TimeOfDay);
            var startOfDay = dateValue.Date;
            var dayOfWeek = dateValue.DayOfWeek;
            // Query the WorkTrackingLogs where the work is late
            var workLogs = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .Where(w => w.HasCheckedOutAEarly == true
                && w.WorkDate == DateOnly.FromDateTime(startOfDay))
                .ToListAsync();
            var lateCheckoutWorkLogs = new List<WorkTrackingLog>();

            foreach (var log in workLogs)
            {
                var workShift = log.User?.WorkShift;

                if (workShift != null && workShift.IsComplex)
                {
                    // Fetch the WorkShiftDetail for the specific day
                    var todayShift = workShift.WorkShiftDetails.FirstOrDefault(d => d.Day == dayOfWeek);
                    if (todayShift != null)
                    {
                        // Validate against the day's specific start and end time
                        if (TimeOnly.FromDateTime(log.WorkTimeStart) >= todayShift.StartTime && TimeOnly.FromDateTime(log.WorkTimeStart) <= todayShift.EndTime)
                        {
                            lateCheckoutWorkLogs.Add(log);
                        }
                    }
                }
                
            }

            return lateCheckoutWorkLogs;
        }
        public async Task<WorkLogValidationResponse> GetWorkLogsByDateAsync(string userId, DateTime date)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.");
            }
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            // Query the WorkTrackingLogs based on the selected date and user
            var workLog = await _context.WorkTrackingLogs
                .Include(w => w.User)
                .ThenInclude(u => u.WorkShift)
                .ThenInclude(ws => ws.WorkShiftDetails)
                .Where(w => w.UserId == userId && w.WorkDate == DateOnly.FromDateTime(startOfDay))
                .FirstOrDefaultAsync();

            if (workLog == null)
            {
                throw new KeyNotFoundException("No work logs found for the given date.");
            }

            var result = _mapper.Map<WorkTrackingLogDTO>(workLog);

            // Return WorkLogValidationResponse
            var response = new WorkLogValidationResponse(true, "Work log fetched successfully.", result);
            return response;
        }
    }
}
