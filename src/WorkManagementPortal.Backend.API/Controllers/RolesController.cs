using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkManagementPortal.Backend.Infrastructure.Dtos.Roles;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Services;

namespace WorkManagementPortal.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
     // Ensuring that only Admins can perform these operations.
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
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = _roleManager.Roles;  // Get all roles from the RoleManager.
            var roleList = new List<RolesListDto>();

            // Add each role (Id and Name) to the list.
            foreach (var role in roles)
            {
                roleList.Add(new RolesListDto
                {
                    Id = role.Id,
                    RoleName = role.Name
                });
            }

            return Ok(roleList); 
        }

        // 2. **Create (Add) Role**
        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is required.");
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
            {
                return BadRequest("Role already exists.");
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' created successfully.");
            }

            return BadRequest(result.Errors);
        }

        // 3. **Update Role Name**
        [HttpPut]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.NewRoleName))
            {
                return BadRequest("Role update data is incomplete.");
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            role.Name = model.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return Ok($"Role '{role.Name}' updated to '{model.NewRoleName}' successfully.");
            }

            return BadRequest(result.Errors);
        }

        // 4. **Delete Role**
        [HttpDelete]
        [Route("DeleteRole/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Role ID is required.");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            // Optionally, check if any users are assigned to this role
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Count > 0)
            {
                return BadRequest("Cannot delete a role that is assigned to users.");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok($"Role '{role.Name}' deleted successfully.");
            }

            return BadRequest(result.Errors);
        }
    }

}
