using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IWorkTrackingRepository : IGenericRepository<WorkTrackingLog, int>
    {
        Task<WorkLogValidationResponse> ClockInAsync(string userId);
        Task<WorkLogValidationResponse> ClockOutAsync(int workLogId);
        Task<PauseLogValidationResponse> StartPauseAsync(StartPauseDto startPauseDto);
        Task<PauseLogValidationResponse> EndPauseAsync(int workLogId);
        Task<List<WorkTrackingLogDTO>> GetFinishedWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetOutofScheduleFinishedWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetPausedWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetOutofSchedulePausedWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetOutofScheduleActiveWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetActiveWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetEarlyCheckoutWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLogDTO>> GetLateCheckInWorkLogsAsync(DateTime? date);
        Task<WorkLogValidationResponse> GetWorkLogsByDateAsync(string userId, DateTime date);

    }
}
