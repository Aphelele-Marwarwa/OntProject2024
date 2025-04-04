using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
namespace Ont3010_Project_YA2024.Data.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _smtpClient = new SmtpClient(options.Value.SmtpServer, options.Value.Port)
            {
                Credentials = new NetworkCredential(options.Value.Username, options.Value.Password),
                EnableSsl = true // Enable SSL for secure connection
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("marwarwa.ap@gmail.com", "Aphelele Marwarwa"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}