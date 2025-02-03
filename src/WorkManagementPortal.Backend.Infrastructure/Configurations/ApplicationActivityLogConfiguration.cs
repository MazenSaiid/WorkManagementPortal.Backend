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
    public class ApplicationActivityLogConfiguration : IEntityTypeConfiguration<ApplicationActivityLog>
    {
        public void Configure(EntityTypeBuilder<ApplicationActivityLog> builder)
        {
            builder.HasOne(l => l.User)
                .WithMany(l=> l.ApplicationActivityLogs)
                .HasForeignKey(l => l.UserId);
        }
    }
}
