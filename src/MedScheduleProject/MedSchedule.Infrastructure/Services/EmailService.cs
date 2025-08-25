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
                client.Authenticate(_emailConfiguration.Email, _emailConfiguration.Password);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendToPatientAppointmentCreated(string patientEmail, string professionalName, 
            string patientName, DateTime appointmentDate, decimal duration)
        {
            var body = GetAppointmentToPatientMessage(professionalName, appointmentDate, duration, patientName);

            await SendEmail(body, "You created a new appointment", patientEmail, patientName);
        }

        private string GetAppointmentToPatientMessage(string professionalName, DateTime appointmentDate, decimal duration, string patientName)
        {
            return $@"<!DOCTYPE html>
            <html lang=""pt-BR"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Confirmação de Consulta</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        margin: 0;
                        padding: 0;
                        background-color: #f9f9f9;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        overflow: hidden;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    }}
                    .content {{
                        padding: 30px;
                    }}
                    .appointment-details {{
                        background-color: #f8f9fa;
                        border-left: 4px solid #667eea;
                        padding: 20px;
                        margin: 20px 0;
                        border-radius: 4px;
                    }}
                    .detail-item {{
                        margin-bottom: 10px;
                        display: flex;
                        align-items: center;
                    }}
                    .detail-icon {{
                        width: 20px;
                        height: 20px;
                        margin-right: 10px;
                        color: #667eea;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 30px;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        text-decoration: none;
                        border-radius: 5px;
                        margin: 20px 0;
                    }}
                    .footer {{
                        background-color: #f8f9fa;
                        padding: 20px;
                        text-align: center;
                        color: #6c757d;
                        font-size: 14px;
                    }}
                    .logo {{
                        font-size: 24px;
                        font-weight: bold;
                        margin-bottom: 10px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
        
                    <div class=""content"">
                        <p>Hi, {patientName}</p>
                        <p>Your appointment was scheduled succesfuly! Here is the details:</p>
            
                        <div class=""appointment-details"">
                            <div class=""detail-item"">
                                <span class=""detail-icon"">📅</span>
                                <strong>Date and Hour:</strong> {appointmentDate}
                            </div>
                            <div class=""detail-item"">
                                <span class=""detail-icon"">⏰</span>
                                <strong>Duração:</strong> {duration} minutos
                            </div>
                            <div class=""detail-item"">
                                <span class=""detail-icon"">👨‍⚕️</span>
                                <strong>Profissional:</strong> Dr. {professionalName}
                            </div>
                        </div>

                        <p>Lembretes importantes:</p>
                        <ul>
                            <li>Arrive 15 minutes early</li>
                            <li>Bring your docs and recently tests</li>
                        </ul>
                    </div>
                </div>
            </body>
            </html>";
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
