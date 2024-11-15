using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class UserRepository : GenericRepository<User, string>, IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;


        public UserRepository(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Helper method to get users by role
        private async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new Exception($"{roleName} role not found");
            }

            return (await _userManager.GetUsersInRoleAsync(role.Name)).ToList();
        }

        public async Task<UserValidationResponse> GetAllSupervisorsAsync()
        {
            try
            {
                var supervisors = await GetUsersByRoleAsync("Supervisor");
                var supervisorsDTOs = _mapper.Map<IEnumerable<UserDto>>(supervisors);
                foreach (var supervisor in supervisorsDTOs)
                {
                    supervisor.RoleName = "Supervisor";  
                }
                return new UserValidationResponse(true, "Supervisors fetched successfully", supervisorsDTOs);
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching supervisors: {ex.Message}");
            }
        }

        public async Task<UserValidationResponse> GetAllTeamLeadersAsync()
        {
            try
            {
                var teamLeaders = await GetUsersByRoleAsync("TeamLead");
                var teamLeadersDTOs = _mapper.Map<IEnumerable<UserDto>>(teamLeaders);
                foreach (var teamLeader in teamLeadersDTOs)
                {
                    teamLeader.RoleName = "TeamLead";
                }
                return new UserValidationResponse(true, "Team leaders fetched successfully", teamLeadersDTOs);
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching team leaders: {ex.Message}");
            }
        }

        public async Task<UserValidationResponse> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await GetUsersByRoleAsync("Employee");
                var employeesDTOs = _mapper.Map<IEnumerable<UserDto>>(employees);
                foreach (var employee in employeesDTOs)
                {
                    employee.RoleName = "Employee";
                }
                return new UserValidationResponse(true, "Employees fetched successfully", employeesDTOs);
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching employees: {ex.Message}");
            }
        }

        public async Task<UserValidationResponse> GetAllSupervisorsAndTheirTeamLeadersAsync()
        {
            try
            {
                var supervisorsWithTeamLeaders = await _context.Users
                    .Where(u => u.TeamLeaderId != null && u.SupervisorId == null) 
                    .Include(u => u.TeamLeader)
                    .ToListAsync();
                var supervisorsWithTeamLeadersDTOs = _mapper.Map<IEnumerable<UserDto>>(supervisorsWithTeamLeaders);
                foreach (var supervisor in supervisorsWithTeamLeadersDTOs)
                {
                    supervisor.RoleName = "Supervisor";
                    supervisor.TeamLeader.RoleName = "TeamLead";
                }
                return new UserValidationResponse(true, "Supervisors and their team leaders fetched successfully", supervisorsWithTeamLeadersDTOs);
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching supervisors and their team leaders: {ex.Message}");
            }
        }

        public async Task<UserValidationResponse> GetAllEmployeesAndTheirSupervisorsAsync()
        {
            try
            {
                var employeesWithSupervisors = await _context.Users
                    .Where(u => u.SupervisorId != null && u.TeamLeaderId != null)
                    .Include(u => u.Supervisor)
                    .ToListAsync();
                var employeesWithSupervisorsDTOs = _mapper.Map<IEnumerable<UserDto>>(employeesWithSupervisors);
                foreach (var employee in employeesWithSupervisorsDTOs)
                {
                    employee.RoleName = "Employee";
                    employee.Supervisor.RoleName = "Supervisor";
                }
                return new UserValidationResponse(true, "Employees and their supervisors fetched successfully", employeesWithSupervisorsDTOs);
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching employees and their supervisors: {ex.Message}");
            }
        }

    }

}
