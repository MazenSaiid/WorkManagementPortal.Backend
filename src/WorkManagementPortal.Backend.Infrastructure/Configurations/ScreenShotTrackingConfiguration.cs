using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Configurations
{
    public class ScreenShotTrackingConfiguration : IEntityTypeConfiguration<ScreenShotTrackingLog>
    {
        public void Configure(EntityTypeBuilder<ScreenShotTrackingLog> builder)
        {
            builder.HasOne(s => s.User)             // Each screenshot log is associated with one user
            .WithMany(u => u.ScreenShotTrackingLogs)  // A user can have many screenshot logs
            .HasForeignKey(s => s.UserId)    // Foreign key in ScreenShotTrackingLog
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete on user deletion


            builder.HasOne(s => s.WorkTrackingLog) // Reference to the related WorkTrackingLog
            .WithMany(w => w.ScreenShotTrackingLogs) // One WorkTrackingLog has many ScreenShotTrackingLogs
            .HasForeignKey(s => s.WorkTrackingLogId); // The foreign key in ScreenShotTrackingLogs
        }
    }
}
