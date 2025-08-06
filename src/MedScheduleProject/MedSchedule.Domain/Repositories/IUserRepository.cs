using MedSchedule.Domain.Aggregates.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetUserById(Guid id);
    }
}
