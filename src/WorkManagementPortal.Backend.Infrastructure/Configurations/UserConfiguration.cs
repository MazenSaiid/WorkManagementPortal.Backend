using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure self-referencing relationships
            builder.HasOne(u => u.Supervisor)
                .WithMany(u => u.Workers)  // One supervisor to many workers
                .HasForeignKey(u => u.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict deletion if a supervisor has workers

            builder.HasOne(u => u.TeamLeader)
                .WithMany(u => u.Supervisors)  // One team leader to many supervisors
                .HasForeignKey(u => u.TeamLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.WorkShift)
                 .WithMany(ws => ws.Users)
                 .HasForeignKey(u => u.WorkShiftId)
                 .OnDelete(DeleteBehavior.SetNull);
            builder.Property(u => u.EmployeeSerialNumber).IsRequired();

        }
    }
}
