using AutoMapper;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkLog;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.API.Mapping
{
    public class WorkShiftMapping : Profile
    {
        public WorkShiftMapping()
        {
            CreateMap<WorkShift, CreateorUpdateWorkShiftDto>().ReverseMap();
            CreateMap<ListWorkShiftDto, WorkShift>().ReverseMap();
            CreateMap<WorkShiftDetail, WorkShiftDetailDto>().ReverseMap();
        }
    }
}
