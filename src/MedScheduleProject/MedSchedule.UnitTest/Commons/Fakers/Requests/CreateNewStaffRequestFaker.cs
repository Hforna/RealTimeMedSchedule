using Bogus;
using MedSchedule.Application.Requests;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Fakers.Requests
{
    public static class CreateNewStaffRequestFaker
    {
        public static CreateNewStaffRequest Generate()
        {
            var startHours = new Faker().Random.Int(00, 23);
            var startMinutes = new Faker().Random.Int(1, 59);
            var endHours = new Faker().Random.Int(startHours, 23);
            var minMinutes = startMinutes + 1;
            var endMinutes = startHours == endHours 
                ? new Faker().Random.Int(minMinutes, 59) 
                : new Faker().Random.Int(0, 59);

            var workShift = new WorkShiftRequest() {
                StartHours = startHours, EndHours = endHours,
                StartMinutes = startMinutes, EndMinutes = endMinutes };

            return new Faker<CreateNewStaffRequest>()
                .RuleFor(d => d.Role, f => f.PickRandom<string>(StaffRoles.GetAllRoles()))
                .RuleFor(d => d.UserId, Guid.NewGuid())
                .RuleFor(d => d.WorkShift, workShift);
        }
    }
}
