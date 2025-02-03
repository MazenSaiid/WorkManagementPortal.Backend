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
    public class WorkShiftDetailConfiguration : IEntityTypeConfiguration<WorkShiftDetail>
    {
        public void Configure(EntityTypeBuilder<WorkShiftDetail> builder)
        {
            builder.HasOne(s => s.WorkShift)         
            .WithMany(u => u.WorkShiftDetails)
            .HasForeignKey(s => s.WorkShiftId) 
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(dws => dws.Day)
            .HasConversion<string>();
        }
    }
}
