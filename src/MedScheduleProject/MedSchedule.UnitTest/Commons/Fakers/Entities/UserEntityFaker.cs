using Bogus;
using MedSchedule.Domain.Aggregates.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Fakers.Entities
{
    public static class UserEntityFaker
    {
        public static User Generate()
        {
            return new Faker<User>()
                .RuleFor(d => d.Id, Guid.NewGuid())
                .RuleFor(d => d.Email, f => f.Internet.Email())
                .RuleFor(d => d.FirstName, f => f.Person.FirstName)
                .RuleFor(d => d.LastName, f => f.Person.LastName)
                .RuleFor(d => d.UserName, f => f.Internet.UserName())
                .RuleFor(d => d.PhoneNumber, f => f.Person.Phone);
        }
    }
}
