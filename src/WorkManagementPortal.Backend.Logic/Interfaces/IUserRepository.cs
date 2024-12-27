using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface IUserRepository :IGenericRepository<User,string>
    {
        Task<UserValidationResponse> GetAllSupervisorsAsync(int page, int pageSize, bool fetchAll = false);
        Task<UserValidationResponse> GetAllTeamLeadersAsync(int page, int pageSize, bool fetchAll = false);
        Task<UserValidationResponse> GetAllEmployeesAsync(int page, int pageSize, bool fetchAll = false);
        Task<UserValidationResponse> GetAllSupervisorsAndTheirTeamLeadersAsync(int page, int pageSize, bool fetchAll = false);
        Task<UserValidationResponse> GetAllEmployeesAndTheirSupervisorsAsync(int page, int pageSize, bool fetchAll = false);
        Task<UserValidationResponse> GetAllEmployeesWithSupervisorsAndTeamLeadsAsync(int page, int pageSize, bool fetchAll = false);
        Task<List<User>> GetEmployeesBySupervisorIdAsync(string supervisorId);
        Task<List<User>> GetEmployeesByTeamLeaderIdAsync(string teamLeaderId);
        Task<bool> UpdateUserRolesAsync(User existingUser, UpdateUserDto entity);
        Task<bool> ValidateAndAssignSupervisorAndTeamLeaderAsync(User existingUser, string id, UpdateUserDto entity);
    }
}
