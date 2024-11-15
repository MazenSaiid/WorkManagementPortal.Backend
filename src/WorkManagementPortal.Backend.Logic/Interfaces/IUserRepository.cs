using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IUserRepository :IGenericRepository<User,string>
    {
        Task<UserValidationResponse> GetAllSupervisorsAsync();
        Task<UserValidationResponse> GetAllTeamLeadersAsync();
        Task<UserValidationResponse> GetAllEmployeesAsync();
        Task<UserValidationResponse> GetAllSupervisorsAndTheirTeamLeadersAsync();
        Task<UserValidationResponse> GetAllEmployeesAndTheirSupervisorsAsync();
    }
}
