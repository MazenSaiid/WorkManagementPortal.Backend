using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Logic.Interfaces
{
    public interface INotificationRepository
    {
        Task SendEmailAsync(string email, string htmlContent, string subject);
    }
}
