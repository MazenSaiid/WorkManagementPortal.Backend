using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkManagementPortal.Backend.API.Dtos.Account;
using WorkManagementPortal.Backend.API.Dtos.User;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;
namespace WorkManagementPortal.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Register method to create a user
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountRepository.RegisterAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // Login method to authenticate a user and generate JWT token
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountRepository.LoginAsync(model);
            if (result.Success)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }

        // Get current authenticated user
        [HttpGet]
        [Route("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID from JWT claims
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ValidationResponse(false,"User not authenticated."));

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new ValidationResponse(false, "No token provided."));

            var result = await _accountRepository.GetCurrentUserAsync(userId, token);

            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}

