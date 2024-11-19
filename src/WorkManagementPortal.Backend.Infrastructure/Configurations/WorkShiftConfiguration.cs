using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Infrastructure.Models;

namespace WorkManagementPortal.Backend.Infrastructure.Configurations
{
    public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
    {

        public void Configure(EntityTypeBuilder<WorkShift> builder)
        {
            builder.Property(p => p.ShiftName).IsRequired();
        }
    }
}
