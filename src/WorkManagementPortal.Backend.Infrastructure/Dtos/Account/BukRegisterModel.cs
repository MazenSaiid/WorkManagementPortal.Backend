using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Infrastructure.Dtos.Account
{
    public class BukRegisterModel
    {
        public IFormFile File { get; set; }
        public string RoleName { get; set; } // Optional role name for assignment
        public string? SupervisorId { get; set; }
        public string? TeamLeaderId { get; set; }
        public int? WorkShiftId { get; set; }
    }
}
