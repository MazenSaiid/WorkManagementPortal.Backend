using AutoMapper;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.API.Mapping
{
    public class WorkLogMapping : Profile
    {
        public WorkLogMapping()
        {
            CreateMap<WorkTrackingLog, WorkTrackingLogDTO>().ReverseMap();
            CreateMap<PauseTrackingLog, PauseTrackingLogDTO>().ReverseMap();
        }
    }
}
