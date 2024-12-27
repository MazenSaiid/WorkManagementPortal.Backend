using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.WorkShift;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class WorkShiftValidationRepsonse : ValidationResponse
    {
        public IEnumerable<ListWorkShiftDto> WorkShifts { get; set; }
        // Pagination details
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public WorkShiftValidationRepsonse(bool success, string message, int currentPage = 1, int pageSize = 20, int totalCount = 0, string token = null, IEnumerable<ListWorkShiftDto> workShifts = null) : base(success, message, token)
        {
            WorkShifts = workShifts ?? new List<ListWorkShiftDto>();
            // Set pagination properties
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
