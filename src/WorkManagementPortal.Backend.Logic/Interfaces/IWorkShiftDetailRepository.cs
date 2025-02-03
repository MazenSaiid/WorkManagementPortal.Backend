using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IWorkShiftDetailRepository :  IGenericRepository<WorkShiftDetail, int>
    {
        Task<IEnumerable<WorkShiftDetail>> GetByWorkShiftIdAsync(int id);
    }
}
