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
            var roleNames = new[] { "Admin", "Manager", "Employee", "Supervisor", "TeamLead" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public async Task SeedShifts(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // List of real shifts data (you can replace this with actual data from a source)
            var realShifts = new List<WorkShift>
             {
                 new WorkShift { ShiftName = "English Call Center Morning Shift", ShiftType = ShiftType.Morning, StartTime = new TimeOnly(0,0,0), EndTime = new TimeOnly(8,0,0) },
                 new WorkShift { ShiftName = "Spanish Call Center MidDay Shift", ShiftType = ShiftType.MidDay, StartTime = new TimeOnly(12,0,0), EndTime = new TimeOnly(20,0,0) },
                 new WorkShift { ShiftName = "Application Support Night Shift", ShiftType = ShiftType.Night, StartTime = new TimeOnly(20,0,0), EndTime = new TimeOnly(4,0,0) },
                 new WorkShift { ShiftName = "Italian Call Center Morning Shift", ShiftType = ShiftType.Morning, StartTime =new TimeOnly(8,0,0), EndTime = new TimeOnly(16,0,0) },
                 new WorkShift { ShiftName = "English Call Center MidDay Shift", ShiftType = ShiftType.MidDay, StartTime = new TimeOnly(10,0,0), EndTime = new TimeOnly(18,0,0) }
             };

            int i = 0;
            int totalShifts = realShifts.Count;

            // Using a while loop to insert real shifts one by one
            while (i < totalShifts)
            {
                var shift = realShifts[i];

                // Check if the shift already exists in the database based on ShiftName and ShiftType
                var shiftExists = await dbContext.WorkShifts
                                                  .AnyAsync(s => s.ShiftName == shift.ShiftName && s.ShiftType == shift.ShiftType && s.StartTime == shift.StartTime);

                if (!shiftExists)
                {
                    // Insert the shift into the database
                    await dbContext.WorkShifts.AddAsync(shift);
                    await dbContext.SaveChangesAsync(); // Save the changes to the database after each insert
                }

                i++;  // Increment to move to the next shift
            }
        }

    }
}