using Bogus;
using MedSchedule.Application.Requests;
using MedSchedule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Fakers.Requests
{
    public static class CreateAppointmentRequestFaker
    {
        public static AppointmentRequest Generate()
        {
            var specialties = new List<string>() { "opthalmology", "pediatrics" };

            return new Faker<AppointmentRequest>()
                .RuleFor(d => d.Time, f => f.Date.Future())
                .RuleFor(d => d.SpecialtyName, f => f.PickRandom<string>(specialties))
                .RuleFor(d => d.PriorityLevel, f => f.PickRandom<EPriorityLevel>());
        }
    }
}
