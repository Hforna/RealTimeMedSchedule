using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace MedSchedule.Domain.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetUserById(Guid id);
        public Task<bool> UserStaffExists(Guid userId);
        public Task<Specialty?> SpecialtyByName(string specialty);
        public Task<List<Staff>?> GetStaffsBySpecialty(Guid specialtyId);
        public Task<User?> FindByEmail(string email);
        public Task Add(User user);
        public Task<ProfessionalInfos?> GetProfessionalInfosByStaffId(Guid staffId);
        public Task<Staff?> StaffById(Guid staff);
        public Task<List<Staff>?> GetAllSpecialtyStaffAvaliableByIds(List<Guid> staffIds, DateTime time);
        public IPagedList<Staff> GetAllStaffsPaginated(StaffPaginatedFilterDto dto);
    }
}
