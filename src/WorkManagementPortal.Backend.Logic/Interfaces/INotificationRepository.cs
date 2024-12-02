using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface INotificationRepository
    {
        Task SendPasswordResetEmailAsync(string email, string resetLink);
    }
}
