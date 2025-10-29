using Bogus;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Fakers.Entities
{
    public static class QueueEntityFaker
    {
        public static QueueRoot GenerateRoot()
        {
            return new Faker<QueueRoot>()
                .RuleFor(d => d.QueueDate, f => f.Date.Future())
                .RuleFor(d => d.Id, Guid.NewGuid())
                .RuleFor(d => d.SpecialtyId, Guid.NewGuid());
        }

        public static QueuePosition GeneratePosition()
        {
            return new Faker<QueuePosition>()
                .RuleFor(d => d.QueueId, Guid.NewGuid())
                .RuleFor(d => d.AppointmentId, Guid.NewGuid())
                .RuleFor(d => d.EffectivePosition, 5)
                .RuleFor(d => d.EstimatedMinutes, 10)
                .RuleFor(d => d.Id, Guid.NewGuid());
        }
    }
}
