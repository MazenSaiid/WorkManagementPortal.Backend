using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class NotificationRepository : INotificationRepository
    {
        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            // Implement email sending logic here (this could be via SMTP, SendGrid, etc.)
            // For example, use an email service to send the resetLink to the user's email.
        }
    }
}
