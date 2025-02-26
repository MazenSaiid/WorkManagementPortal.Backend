using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.Account;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Dtos.Account;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;
using WorkManagementPortal.Backend.Logic.Responses;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;

        public AccountRepository(INotificationRepository notificationRepository,UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _notificationRepository = notificationRepository;
        }

        // Check if an email already exists in the system
        public async Task<bool> CheckExistingEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        // Register a new user
        public async Task<ValidationResponse> RegisterAsync(RegisterModel model)
        {
            var emailExists = await CheckExistingEmailAsync(model.Email);
            if (emailExists)
            {
                return new ValidationResponse(false, $"Email {model.Email} is already in use.");
            }
            var employeeSerialNumberExists = await CheckExistingSerialNumberAsync(model.EmployeeSerialNumber);
            if (employeeSerialNumberExists)
            {
                return new ValidationResponse(false, $"Serial Number {model.EmployeeSerialNumber} is already in use.");
            }

            if (!string.IsNullOrEmpty(model.RoleName) && await _roleManager.RoleExistsAsync(model.RoleName))
            {
                var user = new User
                {
                    EmployeeSerialNumber = model.EmployeeSerialNumber,
                    UserName = string.Concat(model.FirstName,".",model.LastName),
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    SupervisorId = !string.IsNullOrEmpty(model.SupervisorId) ? model.SupervisorId :null,
                    TeamLeaderId = !string.IsNullOrEmpty(model.TeamLeaderId) ? model.TeamLeaderId : null,
                    WorkShiftId = model.WorkShiftId > 0  ? model.WorkShiftId : null,
                };
                var result = await _userManager.CreateAsync(user, _configuration["DefaultPassword"]);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    var htmlTemplate = await File.ReadAllTextAsync("Templates/RegisterNewUserEmail.html");

                    var emailContent = htmlTemplate
                        .Replace("{{UserName}}", user.UserName)
                        .Replace("{{UserEmail}}", model.Email)
                        .Replace("{{DefaultPassword}}", _configuration["DefaultPassword"])
                        .Replace("{{PortalLink}}", _configuration["App:PortalLink"])
                        .Replace("{{DownloadAppLink}}", _configuration["App:WPFDownloadLink"]);

                    await _notificationRepository.SendEmailAsync(model.Email, emailContent, _configuration["App:RegisterNewUserSubject"]);
                    return new ValidationResponse(true, "User created successfully!");
                }
                return new ValidationResponse(false, "User creation failed. " + string.Join(" ", result.Errors.Select(e => e.Description)));
            }
            else
            {
                return new ValidationResponse(false, "Role does not exist.");
            }


        }

        private async Task<bool> CheckExistingSerialNumberAsync(int employeeSerialNumber)
        {
            return _userManager.Users.Any(e => e.EmployeeSerialNumber == employeeSerialNumber);
        }

        public async Task<LoginValidationResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Prepare claims for the JWT
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

                // Add roles to the claims
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Generate a JWT token
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3), // Expiration date of the token
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // Return the full response
                return new LoginValidationResponse(
                    success: true,
                    message: "Login successful",
                    token: new JwtSecurityTokenHandler().WriteToken(token),
                    username: user.UserName, // Return the username
                    userId: user.Id, // Return the userId
                    roles: userRoles.ToList(), // Return the roles
                    localSessionExpireDate: DateTime.Now.AddHours(3) // Return the expiration date
                );
            }

            return new LoginValidationResponse(false, "Invalid login attempt.");
        }


        // Get current user by user ID and parse token
        public async Task<ValidationResponse> GetCurrentUserAsync(string userId, string token)
        {
            // Retrieve user by user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ValidationResponse(false, "User not found.");
            }

            // Parse the token to check expiration
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;

            // Return user details along with roles and the token
            return new ValidationResponse(true, "User retrieved successfully", token);
        }

        // Get user roles
        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<ValidationResponse> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ValidationResponse(false, "No user found with that email address.");
            }

            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // send an email to the user with the resetLink here.
            // Generate the password reset link
            var resetLink = $"{_configuration["App:PasswordResetUrl"]}?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";

            // Load and replace placeholders in the HTML template
            var htmlTemplate = await File.ReadAllTextAsync("Templates/ResetPasswordEmail.html");
            var emailContent = htmlTemplate.Replace("{{resetLink}}", resetLink);

            await _notificationRepository.SendEmailAsync(email, emailContent, _configuration["App:PasswordResetSubject"]);

            return new ValidationResponse(true, "Password reset link has been sent to your email.");
        }


        public async Task<ValidationResponse> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ValidationResponse(false, "No user found with that email address.");
            }

            // Use the token to reset the user's password
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                return new ValidationResponse(true, "Your password has been reset successfully.");
            }

            // If the result contains errors, return them as a string
            return new ValidationResponse(false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<ValidationResponse> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null)
            {
                return new ValidationResponse(false, "No user found with that email address.");
            }

            // Use the token to reset the user's password
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return new ValidationResponse(true, "Your password has been reset successfully.");
            }

            // If the result contains errors, return them as a string
            return new ValidationResponse(false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

}
