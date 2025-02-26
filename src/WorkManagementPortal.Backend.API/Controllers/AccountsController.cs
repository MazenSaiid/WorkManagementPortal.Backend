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
using WorkManagementPortal.Backend.Infrastructure.Dtos.Account;
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
   
        [HttpPost]
        [Route("BulkUpload")]
        public async Task<IActionResult> BulkUpload([FromForm] BukRegisterModel bukRegisterModel)
        {
            if (bukRegisterModel.File == null || bukRegisterModel.File.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            try
            {
                using var stream = new StreamReader(bukRegisterModel.File.OpenReadStream());
                var csvContent = await stream.ReadToEndAsync();

                // Assuming CSV headers: FirstName, LastName, Email, PhoneNumber, EmployeeSerialNumber
                var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var users = new List<RegisterModel>();

                foreach (var line in lines.Skip(1)) // Skip header row
                {
                    var values = line.Split(',');
                    if (values.Length < 5) continue; // Skip invalid rows

                    users.Add(new RegisterModel
                    {
                        EmployeeSerialNumber = int.Parse(values[0]),
                        FirstName = values[1].Trim(),
                        LastName = values[2].Trim(),
                        Email = values[3].Trim(),
                        PhoneNumber = values[4].Trim(),
                        WorkShiftId = bukRegisterModel?.WorkShiftId,
                        RoleName = bukRegisterModel.RoleName,
                        SupervisorId = bukRegisterModel?.SupervisorId,
                        TeamLeaderId = bukRegisterModel?.TeamLeaderId
                    });
                }

                var results = new List<ValidationResponse>();

                foreach (var user in users)
                {
                    var result = await _accountRepository.RegisterAsync(user);
                    results.Add(result);
                }

                var successCount = results.Count(r => r.Success);
                var failureCount = results.Count(r => !r.Success);

                return Ok(new
                {
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Results = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing bulk upload: {ex.Message}");
            }
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
        [HttpPost]
        [Route("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordDto requestPasswordReset)
        {
            if (string.IsNullOrEmpty(requestPasswordReset.Email))
                return BadRequest(new ValidationResponse(false, "Email is required."));

            var result = await _accountRepository.RequestPasswordResetAsync(requestPasswordReset.Email);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (string.IsNullOrEmpty(changePasswordDto.Email) || string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
                return BadRequest(new ValidationResponse(false, "Email, old password, and new password are required."));

            var result = await _accountRepository.ChangePasswordAsync(changePasswordDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrEmpty(resetPasswordDto.Email) || string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
                return BadRequest(new ValidationResponse(false, "Email, token, and new password are required."));

            var result = await _accountRepository.ResetPasswordAsync(resetPasswordDto.Email, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

