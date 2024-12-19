using AutoMapper;
using WorkManagementPortal.Backend.Infrastructure.Dtos.ScreenShot;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.API.Mapping
{
    public class ScreenShotMapping : Profile
    {
        public ScreenShotMapping()
        {
            CreateMap<ScreenShotLogDto,ScreenShotTrackingLog>().ReverseMap();
        }
    }
}
