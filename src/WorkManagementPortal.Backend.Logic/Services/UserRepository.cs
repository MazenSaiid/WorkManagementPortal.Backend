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
using WorkManagementPortal.Backend.Infrastructure.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Enums;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;
using WorkManagementPortal.Backend.Logic.Responses.PaginatedResponses;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class UserRepository : GenericRepository<User, string>, IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IPaginationHelper<User> _paginationHelper;

        public UserRepository(IPaginationHelper<User> paginationHelper, ApplicationDbContext context,UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper) : base(paginationHelper, context)
        {
            _mapper = mapper;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _paginationHelper = paginationHelper;
        }

        // Helper method to get users by role
        private IQueryable<User> GetUsersByRole(string roleName)
        {
            // Get the role from RoleManager
            var role = _roleManager.FindByNameAsync(roleName).Result; // Synchronously fetch the role
            if (role == null)
            {
                throw new Exception($"{roleName} role not found");
            }

            // Use IdentityUserRole to get users for this role
            var userRoles = _context.Set<IdentityUserRole<string>>()
                                    .Where(ur => ur.RoleId == role.Id)
                                    .Select(ur => ur.UserId); // Get the userIds for this role

            // Query the users by the role's userIds
            return _userManager.Users.Where(u => userRoles.Contains(u.Id));
        }

        public async Task<ValidationResponse> GetAllSupervisorsAsync(int page, int pageSize, bool fetchAll = false)
        {
            try
            {
                var supervisorRole = UserRoles.Supervisor.ToString();

                // Get the list of users with the supervisor role
                var supervisors = GetUsersByRole(supervisorRole);

                if (fetchAll)
                {
                    // If fetchAll is true, we return all supervisors without pagination
                    var allSupervisors = supervisors.ToList();
                    var allSupervisorsDTOs = _mapper.Map<IEnumerable<UserDto>>(allSupervisors);

                    // Set the role for each supervisor
                    foreach (var supervisor in allSupervisorsDTOs)
                    {
                        supervisor.RoleName = supervisorRole;
                    }

                    return new UserValidationResponse(true, "All supervisors fetched successfully", null, allSupervisorsDTOs);
                }
                else
                {
                    // If pagination is needed, apply pagination
                    var paginatedResult = await _paginationHelper.GetPagedResult(supervisors, page, pageSize);
                    var supervisorsDTOs = _mapper.Map<IEnumerable<UserDto>>(paginatedResult.Items);

                    // Set the role for each supervisor
                    foreach (var supervisor in supervisorsDTOs)
                    {
                        supervisor.RoleName = supervisorRole;
                    }

                    return new UserValidationPaginatedResponse(true, "Supervisors fetched successfully", page, pageSize, paginatedResult.TotalCount, null, supervisorsDTOs);
                }
            }
            catch (Exception ex)
            {
                // Return an error response in case of failure
                return new UserValidationResponse(false, $"Error fetching supervisors: {ex.Message}");
            }
        }


        public async Task<ValidationResponse> GetAllTeamLeadersAsync(int page, int pageSize, bool fetchAll = false)
        {
            try
            {
                var teamLeadRole = UserRoles.TeamLead.ToString();
                var teamLeaders = GetUsersByRole(teamLeadRole);

                if (fetchAll)
                {
                    // Fetch all team leaders without pagination
                    var allTeamLeaders = teamLeaders.ToList();
                    var allTeamLeadersDTOs = _mapper.Map<IEnumerable<UserDto>>(allTeamLeaders);

                    foreach (var teamLeader in allTeamLeadersDTOs)
                    {
                        teamLeader.RoleName = teamLeadRole;
                    }

                    return new UserValidationResponse(true, "All team leaders fetched successfully", null, allTeamLeadersDTOs);
                }
                else
                {
                    // Apply pagination
                    var paginatedResult = await _paginationHelper.GetPagedResult(teamLeaders, page, pageSize);
                    var teamLeadersDTOs = _mapper.Map<IEnumerable<UserDto>>(paginatedResult.Items);

                    foreach (var teamLeader in teamLeadersDTOs)
                    {
                        teamLeader.RoleName = teamLeadRole;
                    }

                    return new UserValidationPaginatedResponse(true, "Team leaders fetched successfully", page, pageSize, paginatedResult.TotalCount, null, teamLeadersDTOs);
                }
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching team leaders: {ex.Message}");
            }
        }

        public async Task<ValidationResponse> GetAllEmployeesAsync(int page, int pageSize, bool fetchAll = false)
        {
            try
            {
                var employeeRole = UserRoles.Employee.ToString();
                var employees = GetUsersByRole(employeeRole);

                if (fetchAll)
                {
                    // Fetch all employees without pagination
                    var allEmployees = employees.ToList();
                    var allEmployeesDTOs = _mapper.Map<IEnumerable<UserDto>>(allEmployees);

                    foreach (var employee in allEmployeesDTOs)
                    {
                        employee.RoleName = employeeRole;
                    }

                    return new UserValidationResponse(true, "All employees fetched successfully", null, allEmployeesDTOs);
                }
                else
                {
                    // Apply pagination
                    var paginatedResult = await _paginationHelper.GetPagedResult(employees, page, pageSize);
                    var employeesDTOs = _mapper.Map<IEnumerable<UserDto>>(paginatedResult.Items);

                    foreach (var employee in employeesDTOs)
                    {
                        employee.RoleName = employeeRole;
                    }

                    return new UserValidationPaginatedResponse(true, "Employees fetched successfully", page, pageSize, paginatedResult.TotalCount, null, employeesDTOs);
                }
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching employees: {ex.Message}");
            }
        }

        public async Task<ValidationResponse> GetAllSupervisorsAndTheirTeamLeadersAsync(int page, int pageSize, bool fetchAll = false)
        {
            try
            {
                // Query the users with TeamLeaderId and SupervisorId condition without eager loading
                var supervisorsWithTeamLeadersQuery = _context.Users
                    .Where(u => u.TeamLeaderId != null && u.SupervisorId == null)
                    .Join(_context.Users, u => u.TeamLeaderId, tl => tl.Id, (supervisor, teamLeader) => new
                    {
                        Supervisor = supervisor,
                        TeamLeader = teamLeader
                    });

                if (fetchAll)
                {
                    // Fetch all supervisors and their team leaders without pagination
                    var allSupervisorsWithTeamLeaders = supervisorsWithTeamLeadersQuery.ToList();
                    var allSupervisorsWithTeamLeadersDTOs = _mapper.Map<IEnumerable<UserDto>>(allSupervisorsWithTeamLeaders.Select(x => x.Supervisor));

                    foreach (var supervisor in allSupervisorsWithTeamLeadersDTOs)
                    {
                        supervisor.RoleName = UserRoles.Supervisor.ToString();
                        supervisor.TeamLeader.RoleName = UserRoles.TeamLead.ToString();
                    }

                    return new UserValidationResponse(true, "All supervisors and their team leaders fetched successfully", null, allSupervisorsWithTeamLeadersDTOs);
                }
                else
                {
                    // Apply pagination
                    var paginatedResult = await _paginationHelper.GetPagedResult(supervisorsWithTeamLeadersQuery, page, pageSize);

                    var supervisorsWithTeamLeadersDTOs = _mapper.Map<IEnumerable<UserDto>>(paginatedResult.Items.Select(x => x.Supervisor));

                    foreach (var supervisor in supervisorsWithTeamLeadersDTOs)
                    {
                        supervisor.RoleName = UserRoles.Supervisor.ToString();
                        supervisor.TeamLeader.RoleName = UserRoles.TeamLead.ToString();
                    }

                    return new UserValidationPaginatedResponse(true, "Supervisors and their team leaders fetched successfully", page, pageSize, paginatedResult.TotalCount, null, supervisorsWithTeamLeadersDTOs);
                }
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching supervisors and their team leaders: {ex.Message}");
            }
        }

        public async Task<ValidationResponse> GetAllEmployeesAndTheirSupervisorsAsync(int page, int pageSize, bool fetchAll = false)
        {
            try
            {
                // Query employees with SupervisorId and TeamLeaderId condition without eager loading
                var employeesWithSupervisorsQuery = _context.Users
                    .Where(u => u.SupervisorId != null && u.TeamLeaderId != null)
                    .Join(_context.Users, u => u.SupervisorId, sup => sup.Id, (employee, supervisor) => new
                    {
                        Employee = employee,
                        Supervisor = supervisor
                    });

                if (fetchAll)
                {
                    // Fetch all employees and their supervisors without pagination
                    var allEmployeesWithSupervisors = employeesWithSupervisorsQuery.ToList();
                    var allEmployeesWithSupervisorsDTOs = _mapper.Map<IEnumerable<UserDto>>(allEmployeesWithSupervisors.Select(x => x.Employee));

                    foreach (var employee in allEmployeesWithSupervisorsDTOs)
                    {
                        employee.RoleName = UserRoles.Employee.ToString();
                        employee.Supervisor.RoleName = UserRoles.Supervisor.ToString();
                    }

                    return new UserValidationResponse(true, "All employees and their supervisors fetched successfully", null, allEmployeesWithSupervisorsDTOs);
                }
                else
                {
                    // Apply pagination
                    var paginatedResult = await _paginationHelper.GetPagedResult(employeesWithSupervisorsQuery, page, pageSize);

                    var employeesWithSupervisorsDTOs = _mapper.Map<IEnumerable<UserDto>>(paginatedResult.Items.Select(x => x.Employee));

                    foreach (var employee in employeesWithSupervisorsDTOs)
                    {
                        employee.RoleName = UserRoles.Employee.ToString();
                        employee.Supervisor.RoleName = UserRoles.Supervisor.ToString();
                    }

                    return new UserValidationPaginatedResponse(true, "Employees and their supervisors fetched successfully", page, pageSize, paginatedResult.TotalCount, null, employeesWithSupervisorsDTOs);
                }
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching employees and their supervisors: {ex.Message}");
            }
        }

        public async Task<ValidationResponse> GetAllEmployeesWithSupervisorsAndTeamLeadsAsync(int page, int pageSize, bool fetchAll = false)
        {
            try
            {
                // Query employees with SupervisorId and TeamLeaderId condition without eager loading
                var employeesWithSupervisorsAndTeamLeadsQuery = _context.Users
                    .Where(u => u.SupervisorId != null && u.TeamLeaderId != null)
                    .Join(_context.Users, u => u.SupervisorId, sup => sup.Id, (employee, supervisor) => new
                    {
                        Employee = employee,
                        Supervisor = supervisor
                    })
                    .Join(_context.Users, x => x.Employee.TeamLeaderId, tl => tl.Id, (employeeWithSupervisor, teamLeader) => new
                    {
                        Employee = employeeWithSupervisor.Employee,
                        Supervisor = employeeWithSupervisor.Supervisor,
                        TeamLeader = teamLeader
                    });

                if (fetchAll)
                {
                    // Fetch all employees, supervisors, and team leads without pagination
                    var allEmployeesWithSupervisorsAndTeamLeads = employeesWithSupervisorsAndTeamLeadsQuery.ToList();
                    var allEmployeesDTOs = _mapper.Map<IEnumerable<UserDto>>(allEmployeesWithSupervisorsAndTeamLeads.Select(x => x.Employee));

                    foreach (var employee in allEmployeesDTOs)
                    {
                        employee.RoleName = UserRoles.Employee.ToString();
                        employee.Supervisor.RoleName = UserRoles.Supervisor.ToString();
                        employee.TeamLeader.RoleName = UserRoles.TeamLead.ToString();
                    }

                    return new UserValidationResponse(true, "All employees with supervisors and team leads fetched successfully", null, allEmployeesDTOs);
                }
                else
                {
                    // Apply pagination
                    var paginatedResult = await _paginationHelper.GetPagedResult(employeesWithSupervisorsAndTeamLeadsQuery, page, pageSize);

                    var employeesDTOs = _mapper.Map<IEnumerable<UserDto>>(paginatedResult.Items.Select(x => x.Employee));

                    foreach (var employee in employeesDTOs)
                    {
                        employee.RoleName = UserRoles.Employee.ToString();
                        employee.Supervisor.RoleName = UserRoles.Supervisor.ToString();
                        employee.TeamLeader.RoleName = UserRoles.TeamLead.ToString();
                    }

                    return new UserValidationPaginatedResponse(true, "Employees with supervisors and team leads fetched successfully", page, pageSize, paginatedResult.TotalCount, null, employeesDTOs);
                }
            }
            catch (Exception ex)
            {
                return new UserValidationResponse(false, $"Error fetching employees with supervisors and team leads: {ex.Message}");
            }
        }

        public async Task<List<User>> GetEmployeesBySupervisorIdAsync(string supervisorId)
        {
            return await _context.Users
                .Where(u => u.SupervisorId == supervisorId)
                .ToListAsync();
        }

        public async Task<List<User>> GetEmployeesByTeamLeaderIdAsync(string teamLeaderId)
        {
            return await _context.Users
                .Where(u => u.TeamLeaderId == teamLeaderId)
                .ToListAsync();
        }
        // Method to update the user's roles
        public async Task<bool> UpdateUserRolesAsync(User existingUser, UpdateUserDto entity)
        {
            // Update the roles if RoleName is provided
            if (!string.IsNullOrEmpty(entity.RoleName))
            {
                // Validate if the role exists
                var allRoles = await _roleManager.Roles.ToListAsync();
                if (!allRoles.Any(r => r.Name == entity.RoleName))
                {
                    return false; // Invalid role
                }

                // Get the current roles of the user
                var currentRoles = await _userManager.GetRolesAsync(existingUser);

                // Add the new role if it's not already assigned
                if (!currentRoles.Contains(entity.RoleName))
                {
                    await _userManager.AddToRoleAsync(existingUser, entity.RoleName);
                }

                // Remove any roles the user should not have anymore
                foreach (var currentRole in currentRoles)
                {
                    if (currentRole != entity.RoleName)
                    {
                        await _userManager.RemoveFromRoleAsync(existingUser, currentRole);
                    }
                }
            }

            return true; // Roles updated successfully
        }

        // Method to validate and assign supervisor and team leader
        public async Task<bool> ValidateAndAssignSupervisorAndTeamLeaderAsync(User existingUser, string id, UpdateUserDto entity)
        {
            // 1. Validate SupervisorId if provided (It should exist in the database)
            if (!string.IsNullOrEmpty(entity.SupervisorId))
            {
                var supervisorExists = await _userManager.FindByIdAsync(entity.SupervisorId);
                if (supervisorExists == null)
                {
                    return false; // Invalid Supervisor ID
                }

                // Ensure user cannot assign themselves as their own supervisor
                if (entity.SupervisorId == id)
                {
                    return false; // A user cannot be their own supervisor
                }
                existingUser.SupervisorId = entity.SupervisorId;
                var usersAssignedToSupervisor = await _userManager.Users.Where(t => t.SupervisorId == id).ToListAsync();
                foreach (var user in usersAssignedToSupervisor)
                {
                    user.SupervisorId = entity.SupervisorId;
                }
            }

            // 2. Validate TeamLeaderId if provided (It should exist in the database)
            if (!string.IsNullOrEmpty(entity.TeamLeaderId))
            {
                var teamLeaderExists = await _userManager.FindByIdAsync(entity.TeamLeaderId);
                if (teamLeaderExists == null)
                {
                    return false; // Invalid Team Leader ID
                }

                // Ensure user cannot assign themselves as their own team leader
                if (entity.TeamLeaderId == id)
                {
                    return false; // A user cannot be their own team leader
                }

                existingUser.TeamLeaderId = entity.TeamLeaderId;
                var usersAssignedToTeamLeader = await _userManager.Users.Where(t => t.TeamLeaderId == id).ToListAsync();
                foreach (var user in usersAssignedToTeamLeader)
                {
                    user.TeamLeaderId = entity.TeamLeaderId;
                }
            }

            return true; // Supervisor and Team Leader IDs validated and assigned successfully
        }

    }

}
