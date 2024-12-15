using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ScreenShotTrackingLog> ScreenShotTrackingLogs { get; set; }
        public DbSet<WorkShift> WorkShifts { get; set; }
        public DbSet<WorkTrackingLog> WorkTrackingLogs { get; set; }
        public DbSet<LeaveEmployeeRequest> LeaveEmployeeRequests { get; set; }
        public DbSet<PauseTrackingLog> PauseTrackingLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
