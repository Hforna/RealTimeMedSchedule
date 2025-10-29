using Bogus;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Fakers.Entities
{
    public static class SpecialtyEntityFaker
    {
        public static Specialty Generate()
        {
            var specialties = new List<string>() { "opthalmology", "pediatrics" };

            return new Faker<Specialty>()
                .RuleFor(d => d.Id, f => Guid.NewGuid())
                .RuleFor(d => d.Name, f => f.PickRandom<string>(specialties))
                .RuleFor(d => d.MinEmergencySlots, 5)
                .RuleFor(d => d.AvgConsultationTime, 15);
        }

        public static List<Staff> GenerateSpecialtyStaffsInRange(int total, string specialtyName)
        {
            var list = new List<Staff>();

            for(var i = 0; i <= total; i++)
            {
                var staff = StaffEntityFaker.Generate();
                staff.ProfessionalInfos = StaffEntityFaker.GenerateProfessionalInfos();
                staff.ProfessionalInfos.Specialty!.Name = specialtyName;

                list.Add(staff);
            }
            return list;
        }
    }
}
