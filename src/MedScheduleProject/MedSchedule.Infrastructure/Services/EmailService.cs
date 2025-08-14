using MedSchedule.Domain.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace MedSchedule.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }

        public async Task SendEmail(string body, string subject, string toEmail, string toUserName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfiguration.UserName, _emailConfiguration.Email));
            message.To.Add(new MailboxAddress(toUserName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            using(var client = new SmtpClient())
            {
                client.Connect(_emailConfiguration.Provider, _emailConfiguration.Port, SecureSocketOptions.StartTls);
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }

    public class EmailConfiguration
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Provider { get; set; }
        public int Port { get; set; }
    }
}
