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
        Task<List<WorkTrackingLog>> GetFinishedWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLog>> GetPausedWorkLogsAsync(DateTime? date);
        Task<WorkLogValidationResponse> GetWorkLogsByDateAsync(string userId, DateTime date);
        Task<List<WorkTrackingLog>> GetActiveWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLog>> GetEarlyCheckoutWorkLogsAsync(DateTime? date);
        Task<List<WorkTrackingLog>> GetLateCheckInWorkLogsAsync(DateTime? date);

    }
}
