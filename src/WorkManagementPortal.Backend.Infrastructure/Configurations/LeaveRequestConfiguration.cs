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
    public class LeaveRequestConfiguration :IEntityTypeConfiguration<LeaveEmployeeRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveEmployeeRequest> builder)
        {
            // LeaveRequest to User relationship
            builder.HasOne(l => l.User)
                .WithMany(l =>l.LeaveEmployeeRequests)
                .HasForeignKey(l => l.UserId);
        }
    }
}
