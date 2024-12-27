using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Logic.Responses.PaginatedResponses
{
    public class UserValidationPaginatedResponse : ValidationResponse
    {
        // The list of users to be returned
        public IEnumerable<UserDto> Users { get; set; }
        // Pagination details
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public UserValidationPaginatedResponse(bool success, string message, int currentPage = 1, int pageSize = 20, int totalCount = 0, string token = null, IEnumerable<UserDto> users = null) : base(success, message, token)
        {
            // If no users are provided, set to an empty list to avoid null reference issues
            Users = users ?? new List<UserDto>();

            // Set pagination properties
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }

}
