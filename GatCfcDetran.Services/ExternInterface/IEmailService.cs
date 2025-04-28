using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace GatCfcDetran.Services.ExternInterface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
