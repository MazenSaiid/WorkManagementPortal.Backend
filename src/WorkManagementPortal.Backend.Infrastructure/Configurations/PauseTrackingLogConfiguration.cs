﻿using Microsoft.EntityFrameworkCore;
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
    public class PauseTrackingLogConfiguration : IEntityTypeConfiguration<PauseTrackingLog>
    {
        public void Configure(EntityTypeBuilder<PauseTrackingLog> builder)
        {
            builder.HasOne(t => t.WorkTrackingLog)
            .WithMany(w => w.PauseTrackingLogs)
            .HasForeignKey(t => t.WorkTrackingLogId)
            .OnDelete(DeleteBehavior.Cascade); ;
        }
    }
}
