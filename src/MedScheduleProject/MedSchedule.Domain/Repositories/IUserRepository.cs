using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using Microsoft.Extensions.Primitives;
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
        public Task<Specialty?> SpecialtyByName(string specialty);
        public Task<List<Staff>?> GetStaffsBySpecialty(Guid specialtyId);
        public Task<User?> FindByEmail(string email);
        public Task Add(User user);
        public Task<Staff?> StaffById(Guid staff);
    }
}
