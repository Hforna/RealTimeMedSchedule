using Bogus;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.Enums;

namespace MedSchedule.UnitTest.Commons.Fakers.Entities;

public static class AppointmentEntityFaker
{
    public static Appointment Generate()
    {
        return new Faker<Appointment>()
            .RuleFor(d => d.AppointmentStatus, f => f.PickRandom<EAppointmentStatus>())
            .RuleFor(d => d.PatientId, Guid.NewGuid())
            .RuleFor(d => d.StaffId, Guid.NewGuid())
            .RuleFor(d => d.PriorityScore, f => f.Random.Int(1, 10))
            .RuleFor(d => d.Duration, f => f.Random.Int(1, 59))
            .RuleFor(d => d.CheckInDate, DateTime.UtcNow);
    }
}