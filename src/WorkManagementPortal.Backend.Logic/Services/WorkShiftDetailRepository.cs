using Microsoft.EntityFrameworkCore;
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
    public class WorkShiftDetailRepository : GenericRepository<WorkShiftDetail, int>, IWorkShiftDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public WorkShiftDetailRepository(IPaginationHelper<WorkShiftDetail> paginationHelper, ApplicationDbContext context) : base(paginationHelper, context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkShiftDetail>> GetByWorkShiftIdAsync(int id)
        {
          return await _context.WorkShiftDetails.Where(w=>w.WorkShiftId == id).ToListAsync();
        }
    }
}
