using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Services
{
    public interface IEmailService
    {
        public Task SendEmail(string body, string subject, string toEmail, string toUserName);
        public Task SendToPatientAppointmentCreated(string patientEmail, string professionalName, string patientName, DateTime appointmentDate, decimal duration);
    }
}
