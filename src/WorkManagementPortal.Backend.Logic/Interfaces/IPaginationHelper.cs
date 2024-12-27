using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IPaginationHelper<T> where T : class
    {
        Task<PagedResult<T>> GetPagedResult<T>(IQueryable<T> query, int page, int pageSize);
    }

}
