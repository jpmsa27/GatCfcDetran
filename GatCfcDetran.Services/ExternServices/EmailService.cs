using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GatCfcDetran.Services.ExternInterface;

namespace GatCfcDetran.Services.ExternServices
{
    public class EmailService(IConfiguration configuration, SmtpClient smtpClient) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly SmtpClient _smtpClient = smtpClient;

        public async Task SendEmailAsync(string to, string subject, string body)
        {

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["SMTPCredentials:User"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(to);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
