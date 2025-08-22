using MedSchedule.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons
{
    public static class RolesRandomCase
    {
        public static IEnumerable<object[]> GetRolesRandomlyCase()
        {
            foreach(var role in StaffRoles.GetAllRoles())
            {
                yield return new object[] { role };
                yield return new object[] { role.ToLower() };
                yield return new object[] { role.ToUpper() };
            }
        }
    }
}
