using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WorkManagementPortal.Backend.Logic.Interfaces;

namespace WorkManagementPortal.Backend.Logic.Services
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IConfiguration _configuration;
        public NotificationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string htmlContent, string subject)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(senderEmail, password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:Email"], "Innovative BPO"), // Optional: Set the display name
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }

    }
}
