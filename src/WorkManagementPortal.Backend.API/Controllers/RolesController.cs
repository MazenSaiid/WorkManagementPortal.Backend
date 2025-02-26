using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WorkManagementPortal.Backend.Infrastructure.Dtos.Roles;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;
using WorkManagementPortal.Backend.Logic.Services;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // 1. **Get all roles**
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            try
            {
                var roles = _roleManager.Roles;
                var roleList = new List<RolesListDto>();

                foreach (var role in roles)
                {
                    roleList.Add(new RolesListDto
                    {
                        Id = role.Id,
                        RoleName = role.Name
                    });
                }
                if (fetchAll)
                {
                    var response = new RolesValidationResponse(
                        true,
                        "Roles retrieved successfully.",
                        roles: roleList
                    );
                    return Ok(response);

                }
                else
                {
                    var paginatedResult = roleList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    var totalCount = roleList.Count;
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                    var response = new RolesValidationPaginatedResponse(
                                   success: true,
                                   message: "User count per role retrieved successfully.",
                                   currentPage: page,
                                   pageSize: pageSize,
                                   totalCount: totalCount,
                                   roles: paginatedResult);

                    return Ok(response);
                }

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error fetching all roles: {ex.Message}");
            }
            
        }

        // 2. **Create Role**
        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            if (string.IsNullOrEmpty(roleCreateDto.RoleName))
            {
                var response = new RolesValidationResponse(
                    false,
                    "Role name is required."
                );
                return BadRequest(response);
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleCreateDto.RoleName);
            if (roleExist)
            {
                var response = new RolesValidationResponse(
                    false,
                    "Role already exists."
                );
                return BadRequest(response);
            }

            var role = new IdentityRole(roleCreateDto.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                var response = new RolesValidationResponse(
                    true,
                    $"Role '{roleCreateDto.RoleName}' created successfully."
                );
                return Ok(response);
            }

            var responseFailure = new RolesValidationResponse(
                false,
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
            return BadRequest(responseFailure);
        }

        // 3. **Update Role Name**
        [HttpPut]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.RoleName))
            {
                var response = new RolesValidationResponse(
                    false,
                    "Role update data is incomplete."
                );
                return BadRequest(response);
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                var response = new RolesValidationResponse(
                    false,
                    "Role not found."
                );
                return NotFound(response);
            }

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var response = new RolesValidationResponse(
                    true,
                    $"Role '{role.Name}' updated to '{model.RoleName}' successfully."
                );
                return Ok(response);
            }

            var responseFailure = new RolesValidationResponse(
                false,
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
            return BadRequest(responseFailure);
        }

        // 4. **Delete Role**
        [HttpDelete]
        [Route("DeleteRole/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                var response = new RolesValidationResponse(
                    false,
                    "Role ID is required."
                );
                return BadRequest(response);
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                var response = new RolesValidationResponse(
                    false,
                    "Role not found."
                );
                return NotFound(response);
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Count > 0)
            {
                var response = new RolesValidationResponse(
                    false,
                    "Cannot delete a role that is assigned to users."
                );
                return BadRequest(response);
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                var response = new RolesValidationResponse(
                    true,
                    $"Role '{role.Name}' deleted successfully."
                );
                return Ok(response);
            }

            var responseFailure = new RolesValidationResponse(
                false,
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
            return BadRequest(responseFailure);
        }
        [HttpGet]
        [Route("GetUserCountPerRole")]
        public async Task<IActionResult> GetUserCountPerRole(int page = 1, int pageSize = 20, bool fetchAll = false)
        {
            try
            {
                var roles = _roleManager.Roles;
                var roleUserCounts = new List<RolesListDto>();

                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                    roleUserCounts.Add(new RolesListDto
                    {
                        Id = role.Id,
                        RoleName = role.Name,
                        UserCount = usersInRole.Count
                    });
                }
                if (fetchAll)
                {
                    var response = new RolesValidationResponse(
                        true,
                        "User count per role retrieved successfully.",
                        roles: roleUserCounts
                    );
                    return Ok(response);
                }
                else
                {
                    var paginatedResult = roleUserCounts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    var totalCount = roleUserCounts.Count;
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                    var response = new RolesValidationPaginatedResponse(
                                   success: true,
                                   message: "User count per role retrieved successfully.",
                                   currentPage: page,
                                   pageSize: pageSize,
                                   totalCount: totalCount,
                                   roles: paginatedResult);

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error fetching all roles per count: {ex.Message}");
            }

        }
    }


}
