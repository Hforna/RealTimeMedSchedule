using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Services.EmailTemplates
{
    public static class EmailHtmlTemplates
    {
        public static string GetAppointmentToPatientMessage(string professionalName, DateTime appointmentDate, decimal duration, string patientName)
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
}
