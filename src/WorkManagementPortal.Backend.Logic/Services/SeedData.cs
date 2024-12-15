using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkManagementPortal.Backend.Infrastructure.Context;
using WorkManagementPortal.Backend.Infrastructure.Enums;
using WorkManagementPortal.Backend.Infrastructure.Models;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class SeedData : ISeedData
    {
        public async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
             // Get role names from the UserRoles enum
            var roleNames = Enum.GetValues<UserRoles>()
                                .Cast<UserRoles>()
                                .Select(r => r.ToString())
                                .ToArray();

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

    }
}