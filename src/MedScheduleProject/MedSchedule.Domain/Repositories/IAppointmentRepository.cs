using MedSchedule.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        public Task<List<Staff>?> GetAllSpecialtyStaffAvaliableByIds(List<Guid> staffIds, DateTime Time);
    }
}
