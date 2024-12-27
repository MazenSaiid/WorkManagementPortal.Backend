using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Dtos.Roles;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class RolesValidationPaginatedResponse : ValidationResponse
    {
        public IEnumerable<RolesListDto> Roles { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public RolesValidationPaginatedResponse(
            bool success,
            string message,
            int currentPage = 1, int pageSize = 20, int totalCount = 0,
            IEnumerable<RolesListDto> roles = null,
            string token = null)
            : base(success, message, token)
        {
            Roles = roles ?? new List<RolesListDto>();
            // Set pagination properties
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
