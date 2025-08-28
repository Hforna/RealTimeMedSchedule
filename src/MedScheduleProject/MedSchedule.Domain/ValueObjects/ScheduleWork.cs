using MedSchedule.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.ValueObjects
{
    public sealed record ScheduleWork
    {
        public ScheduleWork(int startHours, int startMinutes, int endHours, int endMinutes)
        {
            StartHours = startHours;
            StartMinutes = startMinutes;
            EndHours = endHours;
            EndMinutes = endMinutes;

            Validate();
        }

        public ScheduleWork(int startHours, int startMinutes)
        {
            StartHours = startHours;
            StartMinutes = startMinutes;
        }

        public int StartHours { get; set; }
        public int StartMinutes { get; set; }
        public int EndHours { get; set; }
        public int EndMinutes { get; set; }
        public DateTime AppointmentDate { get; set; }

        public void CalculateTotalTimeForFinish(int avgConsultTime)
        {
            if (avgConsultTime <= 0)
                throw new ArgumentException("avgConsultTime must be positive");

            var totalMinutes = StartMinutes + avgConsultTime;

            if (totalMinutes >= 60)
            {
                var extraHours = totalMinutes / 60;
                var remainingMinutes = totalMinutes % 60;

                EndHours = StartHours + extraHours;
                EndMinutes = remainingMinutes;
            }
            else
            {
                EndHours = StartHours;
                EndMinutes = totalMinutes;
            }

            Validate();
        }

        private void Validate()
        {
            if (StartHours > EndHours || (StartHours == EndHours && StartMinutes >= EndMinutes))
                throw new DomainException("Start time must be higher than end time");

            if ((StartHours > 23 || StartHours < 0) || (EndHours > 23 || EndHours < 0))
                throw new DomainException("Work time hours must be between 0 and 23");

            if ((StartMinutes > 59 || StartMinutes < 0) || (EndMinutes > 59 || EndMinutes < 0))
                throw new DomainException("Work time minutes must be between 0 and 59");
        }

        public Decimal DurationInHours()
        {
            var startTotal = StartHours * 60 + StartMinutes;
            var endTotal = EndHours * 60 + EndMinutes;
            var totalHours = Math.Abs(startTotal - endTotal / 60m);

            return Math.Round(totalHours, 2);
        }
    }
}
