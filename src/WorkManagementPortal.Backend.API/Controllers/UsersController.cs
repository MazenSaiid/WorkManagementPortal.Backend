using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public UsersController(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // Get all supervisors
        [HttpGet("Supervisors")]
        public async Task<IActionResult> GetAllSupervisors()
        {
            var response = await _userRepository.GetAllSupervisorsAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all team leaders
        [HttpGet("TeamLeaders")]
        public async Task<IActionResult> GetAllTeamLeaders()
        {
            var response = await _userRepository.GetAllTeamLeadersAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all employees (excluding supervisors and team leaders)
        [HttpGet("Employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var response = await _userRepository.GetAllEmployeesAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all supervisors and their team leaders
        [HttpGet("SupervisorsAndTeamLeaders")]
        public async Task<IActionResult> GetAllSupervisorsAndTheirTeamLeaders()
        {
            var response = await _userRepository.GetAllSupervisorsAndTheirTeamLeadersAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all employees and their supervisors
        [HttpGet("EmployeesAndSupervisors")]
        public async Task<IActionResult> GetAllEmployeesAndTheirSupervisors()
        {
            var response = await _userRepository.GetAllEmployeesAndTheirSupervisorsAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        // Get all employees and their supervisors
        [HttpGet("EmployeesAndTheirHeads")]
        public async Task<IActionResult> GetAllEmployeesAndHeads()
        {
            var response = await _userRepository.GetAllEmployeesAndHeadsAsync();

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        // Fetching all users)
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (!users.Any())
                {
                    return NotFound(new UserValidationResponse(false, " No Users found"));
                }
                List<UserDto> usersDTOs = new List<UserDto>();
                foreach (var user in users)
                {

                    var usersDTO = _mapper.Map<UserDto>(user);
                    // Get the roles for the user
                    var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Any())
                        {
                        usersDTO.RoleName = roles.FirstOrDefault() ?? "No Role"; 
                        usersDTOs.Add(usersDTO);
                        }
                        else
                        {
                        usersDTO.RoleName = "No Role";
                        }
                    
                }
                    return Ok(new UserValidationResponse(true, "All users fetched successfully", usersDTOs));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching all users: {ex.Message}");
            }
        }

        // Example of GET by id
        [HttpGet]
        [Route("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new UserValidationResponse(false, "User not found"));
                }
                var userDTO = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    userDTO.RoleName = roles.FirstOrDefault() ?? "No Role"; // You can handle multiple roles if necessary
                }
                else
                {
                    userDTO.RoleName = "No Role";
                }
                return Ok(new UserValidationResponse(true, "User fetched successfully", new List<UserDto> { userDTO }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching user by ID: {ex.Message}");
            }
        }

        // Example of PUT (updating an existing user)
        [HttpPut]
        [Route("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto entity)
        {

            try
            {
                if (entity == null || string.IsNullOrEmpty(id))
                {
                    return BadRequest("User data is incorrect");
                }
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }
                // Update roles
                var roleUpdateResult = await UpdateUserRolesAsync(existingUser, entity);
                if (!roleUpdateResult)
                {
                    return BadRequest("Invalid role.");
                }

                // Validate and assign supervisor/team leader
                var supervisorTeamLeaderResult = await ValidateAndAssignSupervisorAndTeamLeaderAsync(existingUser, id, entity);
                if (!supervisorTeamLeaderResult)
                {
                    return BadRequest("Invalid supervisor or team leader ID.");
                }
                existingUser.UserName = string.Concat(entity.FirstName, ".", entity.LastName);
                _mapper.Map(entity, existingUser);
                await  _userManager.UpdateAsync(existingUser);
                return Ok(new UserValidationResponse(true, "User updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user: {ex.Message}");
            }
        }

        // Example of DELETE (removing a user)
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                await _userRepository.DeleteAsync(id); // Deletes the user
                return Ok(new UserValidationResponse(true, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Private method to update the user's roles
        private async Task<bool> UpdateUserRolesAsync(User existingUser, UpdateUserDto entity)
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

        // Private method to validate and assign supervisor and team leader
        private async Task<bool> ValidateAndAssignSupervisorAndTeamLeaderAsync(User existingUser, string id, UpdateUserDto entity)
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
