﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.API.Dtos.Account;
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

        public AccountRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // Check if an email already exists in the system
        public async Task<bool> CheckExistingEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        // Register a new user
        public async Task<UserResponse> RegisterAsync(RegisterModel model)
        {
            var emailExists = await CheckExistingEmailAsync(model.Email);
            if (emailExists)
            {
                return new UserResponse(false, "Email is already in use.");
            }

            var user = new User {
                UserName = string.Concat(model.FirstName,model.LastName), 
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign role if provided and it exists
                if (!string.IsNullOrEmpty(model.RoleName))
                {
                    if (await _roleManager.RoleExistsAsync(model.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, model.RoleName);
                    }
                    else
                    {
                        return new UserResponse(false, "Role does not exist.");
                    }
                }
                return new UserResponse(true, "User created successfully!");
            }

            return new UserResponse(false, "User creation failed.");
        }

        // Login method to authenticate a user and generate JWT token
        public async Task<UserResponse> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new UserResponse(true, "Login successful", new JwtSecurityTokenHandler().WriteToken(token));
            }

            return new UserResponse(false, "Invalid login attempt.");
        }

        // Get current user by user ID and parse token
        public async Task<UserResponse> GetCurrentUserAsync(string userId, string token)
        {
            // Retrieve user by user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserResponse(false, "User not found.");
            }

            // Parse the token to check expiration
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;

            // Return user details along with roles and the token
            return new UserResponse(true, "User retrieved successfully", token);
        }

        // Get user roles
        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<UserResponse> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserResponse(false, "No user found with that email address.");
            }

            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Send the token via email to the user (you need to implement email sending)
            var resetLink = $"{_configuration["App:PasswordResetUrl"]}?token={token}&email={email}";
            // Ideally, you would send an email to the user with the resetLink here.
            await SendPasswordResetEmailAsync(email, resetLink);

            return new UserResponse(true, "Password reset link has been sent to your email.");
        }

        private async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            // Implement email sending logic here (this could be via SMTP, SendGrid, etc.)
            // For example, use an email service to send the resetLink to the user's email.
        }

        public async Task<UserResponse> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserResponse(false, "No user found with that email address.");
            }

            // Use the token to reset the user's password
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                return new UserResponse(true, "Your password has been reset successfully.");
            }

            // If the result contains errors, return them as a string
            return new UserResponse(false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

    }

}