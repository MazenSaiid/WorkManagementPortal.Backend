﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Configurations
{
    public class WorkLogConfiguration : IEntityTypeConfiguration<WorkTrackingLog>
    {
        public void Configure(EntityTypeBuilder<WorkTrackingLog> builder)
        {
            builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);
        }
    }
}
