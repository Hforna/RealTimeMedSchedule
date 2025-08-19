using MedSchedule.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.ValueObjects
{
    [Owned]
    public sealed record WorkShift
    {
        public WorkShift(int startHours, int endHours, int startMinutes, int endMinutes)
        {
            StartHours = startHours;
            EndHours = endHours;
            StartMinutes = startMinutes;
            EndMinutes = endMinutes;

            Validate();
        }

        public int StartHours { get; private set; }
        public int EndHours { get; private set; }
        public int StartMinutes { get; private set; }
        public int EndMinutes { get; private set; }

        public void Validate()
        {
            if (StartHours > EndHours || (StartHours == EndHours && StartMinutes >= EndMinutes))
                throw new DomainException("Start time must be higher than end time");

            if ((StartHours > 23 || StartHours < 0) || (EndHours > 23 || EndHours < 0))
                throw new DomainException("Work time hours must be between 0 and 23");

            if ((StartMinutes > 59 || StartMinutes < 0) || (EndMinutes > 59 || EndMinutes < 0))
                throw new DomainException("Work time minutes must be between 0 and 59");
        }

        public bool IsTimeBetweenShift(DateTime time)
        {
            var startTime = new TimeSpan(StartHours, StartMinutes, 0);
            var endTime = new TimeSpan(EndHours, EndMinutes, 0);
            var checkTime = new TimeSpan(time.Hour, time.Minute, 0);

            return checkTime >= startTime && checkTime <= endTime;
        }
    }
}
