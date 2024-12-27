using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class ScreenShotRepository : GenericRepository<ScreenShotTrackingLog, int>, IScreenShotRepository
    {
        public ScreenShotRepository(IPaginationHelper<ScreenShotTrackingLog> paginationHelper, ApplicationDbContext context) : base(paginationHelper, context)
        {
        }
    }
}
