using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.AggregatesModel.UserAggregate
{
    public class Staff : Entity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Role { get; set; }
        public required WorkShift WorkShift { get; set; }
        public ProfessionalInfos? ProfessionalInfos { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = [];
    }

    public class ProfessionalInfos : Entity
    {
        public Guid StaffId { get; set; }
        public Staff? Staff { get; set; }
        public Guid SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
        public int TotalServices { get; set; } = 0;
        public int? AvgConsultationTime { get; set; }
        public int MaxPriorityLevel { get; set; } = 0;
        public IList<Appointment> Appointments { get; set; } = [];
    }

    public class StaffRoles
    {
        public const string Professional = "professional";
        public const string Admin = "admin";

        public static bool IsValidRole(string role)
        {
            var contains = GetAllRoles().Any(d => d.Equals(role, StringComparison.OrdinalIgnoreCase));

            return contains;
        }

        public static List<string> GetAllRoles()
        {
            return GetAllConstants().Select(d => d.GetRawConstantValue() as string).ToList()!;
        }

        private static IEnumerable<FieldInfo> GetAllConstants()
        {
            var fieldInfos = typeof(StaffRoles)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }
    }

    public class Specialty : Entity
    {
        public string Name { get; set; }
        public int AvgConsultationTime { get; set; }
        public int MinEmergencySlots { get; set; }
    }
}
