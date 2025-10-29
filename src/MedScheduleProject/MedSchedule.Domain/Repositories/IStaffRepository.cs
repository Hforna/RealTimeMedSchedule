using MedSchedule.Domain.AggregatesModel.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IStaffRepository
    {
        public Task<Staff?> GetStaffByUserId(Guid userId);
    }
}
