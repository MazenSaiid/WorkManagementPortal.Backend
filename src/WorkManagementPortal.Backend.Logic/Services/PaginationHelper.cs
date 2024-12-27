using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class PaginationHelper<T> : IPaginationHelper<T> where T : class
    {
        public async Task<PagedResult<T>> GetPagedResult<T>(IQueryable<T> query, int page, int pageSize)
        {
            // Validate the page and pageSize values
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page and pageSize must be greater than 0.");
            }

            // Get the total count of items
            var totalCount = await query.CountAsync();

            // Get the paginated data (skip and take based on page and pageSize)
            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Return the result
            return new PagedResult<T>
            {
                TotalCount = totalCount,
                Items = data
            };
        }
    }


}
