using TMS.NotifyService.Abstractions;
using System.Net;
using System.Net.Mail;

namespace TMS.NotifyService.Email
{
    public class EmailNotifier : INotificationSender
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromAddress;

        public EmailNotifier(string smtpHost, int smtpPort, string smtpUser, string smtpPassword, string fromAddress)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPassword = smtpPassword;
            _fromAddress = fromAddress;
        }

        public async Task SendAsync(string email, string message)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true
            };

            var mail = new MailMessage(_fromAddress, email)
            {
                Subject = "Код подтверждения",
                Body = message
            };

            await client.SendMailAsync(mail);
        }
    }
}