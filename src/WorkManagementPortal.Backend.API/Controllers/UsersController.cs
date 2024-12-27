using AutoMapper;
using Azure;
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
using WorkManagementPortal.Backend.Logic.Responses.PaginatedResponses;
using WorkManagementPortal.Backend.Logic.Services;

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
        // Fetching all users)
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            try
            {
                if (fetchAll)
                {
                    // Fetch all users from the repository
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
                            usersDTO.RoleName = roles.FirstOrDefault();
                            usersDTOs.Add(usersDTO);
                        }
                        else
                        {
                            usersDTO.RoleName = "No Role";
                        }
                        usersDTOs.Add(usersDTO);
                    }
                    return Ok(new UserValidationResponse(
                        success: true,
                        message: "All users fetched successfully",
                        null,
                        users: usersDTOs
                    ));
                }
                else
                {
                    // Fetch paginated users from the repository
                    var paginatedResult = await _userRepository.GetAllAsync(page, pageSize);

                    // Check if there are any users
                    if (!paginatedResult.Items.Any())
                    {
                        return NotFound(new UserValidationResponse(false, "No Users found"));
                    }

                    // Convert to DTOs and map user roles
                    List<UserDto> usersDTOs = new List<UserDto>();
                    foreach (var user in paginatedResult.Items)
                    {
                        var usersDTO = _mapper.Map<UserDto>(user);
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Any())
                        {
                            usersDTO.RoleName = roles.FirstOrDefault();
                        }
                        else
                        {
                            usersDTO.RoleName = "No Role";
                        }
                        usersDTOs.Add(usersDTO);
                    }

                    // Return the paginated response
                    return Ok(new UserValidationPaginatedResponse(
                        success: true,
                        message: "All users fetched successfully",
                        currentPage: page,
                        pageSize: pageSize,
                        totalCount: paginatedResult.TotalCount,
                        null,
                        users: usersDTOs
                    ));

                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching all users: {ex.Message}");
            }
        }

        // GET by id
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
                    userDTO.RoleName = roles.FirstOrDefault(); // You can handle multiple roles if necessary
                }
                else
                {
                    userDTO.RoleName = "No Role";
                }
                return Ok(new UserValidationResponse(true, "User fetched successfully", null, new List<UserDto> { userDTO }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching user by ID: {ex.Message}");
            }
        }

        // PUT (updating an existing user)
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
                var roleUpdateResult = await _userRepository.UpdateUserRolesAsync(existingUser, entity);
                if (!roleUpdateResult)
                {
                    return BadRequest("Invalid role.");
                }

                // Validate and assign supervisor/team leader
                var supervisorTeamLeaderResult = await _userRepository.ValidateAndAssignSupervisorAndTeamLeaderAsync(existingUser, id, entity);
                if (!supervisorTeamLeaderResult)
                {
                    return BadRequest("Invalid supervisor or team leader ID.");
                }
                existingUser.UserName = string.Concat(entity.FirstName, ".", entity.LastName);
                _mapper.Map(entity, existingUser);
                await _userManager.UpdateAsync(existingUser);
                return Ok(new UserValidationResponse(true, "User updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user: {ex.Message}");
            }
        }

        // DELETE (removing a user)
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                // Get the user by ID
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new UserValidationResponse(false, "User not found"));
                }

                // Check if the user is an employee (has both SupervisorId and TeamLeaderId)
                if (!string.IsNullOrEmpty(user.SupervisorId) && !string.IsNullOrEmpty(user.TeamLeaderId))
                {
                    await _userRepository.DeleteAsync(id); // Delete the employee
                    return Ok(new UserValidationResponse(true, "User deleted successfully"));
                }

                // Check if the user is a supervisor (has TeamLeaderId but no SupervisorId)
                if (!string.IsNullOrEmpty(user.TeamLeaderId) && string.IsNullOrEmpty(user.SupervisorId))
                {
                    // Check if the supervisor has employees under them
                    var employeesUnderSupervisor = await _userRepository.GetEmployeesBySupervisorIdAsync(user.Id);
                    if (employeesUnderSupervisor.Any())
                    {
                        return BadRequest(new UserValidationResponse(false, "Cannot delete this supervisor as there are employees under them"));
                    }
                }

                // Check if the user is a team leader (has neither SupervisorId nor TeamLeaderId)
                if (string.IsNullOrEmpty(user.SupervisorId) && string.IsNullOrEmpty(user.TeamLeaderId))
                {
                    // Team leaders can be deleted without restriction unless they are still referenced as a SupervisorId or TeamLeaderId
                    var employeesUnderTeamLeaderCheck = await _userRepository.GetEmployeesByTeamLeaderIdAsync(user.Id);
                    var employeesUnderSupervisorCheck = await _userRepository.GetEmployeesBySupervisorIdAsync(user.Id);

                    if (employeesUnderTeamLeaderCheck.Any() || employeesUnderSupervisorCheck.Any())
                    {
                        return BadRequest(new UserValidationResponse(false, "Cannot delete this team leader as there are supervisors under them"));
                    }
                }
                // Proceed with deletion if no references exist
                await _userRepository.DeleteAsync(id);
                return Ok(new UserValidationResponse(true, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get all supervisors
        [HttpGet("Supervisors")]
        public async Task<IActionResult> GetAllSupervisorsPaginated(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            var response = await _userRepository.GetAllSupervisorsAsync(page,pageSize,fetchAll);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all team leaders
        [HttpGet("TeamLeaders")]
        public async Task<IActionResult> GetAllTeamLeadersPaginated(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            var response = await _userRepository.GetAllTeamLeadersAsync(page,pageSize,fetchAll);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all employees (excluding supervisors and team leaders)
        [HttpGet("Employees")]
        public async Task<IActionResult> GetAllEmployeesPaginated(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            var response = await _userRepository.GetAllEmployeesAsync(page,pageSize,fetchAll);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all supervisors and their team leaders
        [HttpGet("SupervisorsAndTeamLeaders")]
        public async Task<IActionResult> GetAllSupervisorsAndTheirTeamLeadersPaginated(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            var response = await _userRepository.GetAllSupervisorsAndTheirTeamLeadersAsync(page,pageSize,fetchAll);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        // Get all employees and their supervisors
        [HttpGet("EmployeesAndSupervisors")]
        public async Task<IActionResult> GetAllEmployeesAndTheirSupervisorsPaginated(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            var response = await _userRepository.GetAllEmployeesAndTheirSupervisorsAsync(page,pageSize,fetchAll);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet]
        [Route("EmployeesWithSupervisorsAndTeamLeadsAsync")]
        public async Task<IActionResult> GetAlEmployeesWithSupervisorsAndTeamLeadsAsync(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            try
            {
                var response = await _userRepository.GetAllEmployeesWithSupervisorsAndTeamLeadsAsync(page, pageSize,fetchAll);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching all users: {ex.Message}");
            }
        }

    }

}
