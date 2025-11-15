using Bogus;
using MedSchedule.Application.Requests;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Fakers.Entities
{
    public static class StaffEntityFaker
    {
        public static Staff Generate()
        {
            var startHours = new Faker().Random.Int(00, 23);
            var startMinutes = new Faker().Random.Int(1, 59);
            var endHours = new Faker().Random.Int(startHours, 23);
            var minMinutes = startMinutes + 1;
            var endMinutes = startHours == endHours
                ? new Faker().Random.Int(minMinutes, 59)
                : new Faker().Random.Int(0, 59);

            var workShift = new WorkShift(
                startHours,
                endHours,
                startMinutes,
                endMinutes);

            var user = UserEntityFaker.Generate();

            return new Faker<Staff>()
                .RuleFor(d => d.WorkShift, workShift)
                .RuleFor(d => d.Role, f => f.PickRandom<string>(StaffRoles.GetAllRoles()))
                .RuleFor(d => d.Id, Guid.NewGuid())
                .RuleFor(d => d.UserId, user.Id)
                .RuleFor(d => d.User, user);
        }

        public static ProfessionalInfos GenerateProfessionalInfos()
        {
            var specialty = SpecialtyEntityFaker.Generate();

            return new Faker<ProfessionalInfos>()
                .RuleFor(d => d.StaffId, Guid.NewGuid())
                .RuleFor(d => d.SpecialtyId, specialty.Id)
                .RuleFor(d => d.Specialty, specialty)
                .RuleFor(d => d.Id, specialty.Id)
                .RuleFor(d => d.TotalServices, 10)
                .RuleFor(d => d.AvgConsultationTime, 15)
                .RuleFor(d => d.MaxPriorityLevel, 2);
        }
    }
}
