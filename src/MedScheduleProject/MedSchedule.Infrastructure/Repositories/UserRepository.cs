using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectDataContext _context;

        public UserRepository(ProjectDataContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> FindByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Email == email);
        }

        public async Task<List<Staff>?> GetStaffsBySpecialty(Guid specialtyId)
        {
            return await _context.Staffs
                .AsNoTracking()
                .Where(d => d.Role == StaffRoles.Professional && d.SpecialtyId == specialtyId)
                .ToListAsync();
        }

        public async Task<User?> GetUserById(Guid id) => await _context.Users.SingleOrDefaultAsync(d => d.Id == id);

        public async Task<Specialty?> SpecialtyByName(string specialty)
        {
            return await _context.Specialties
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Name.ToLower() == specialty.ToLower().Trim());
        }

        public async Task<Staff?> StaffById(Guid staff)
        {
            return await _context.Staffs.SingleOrDefaultAsync(d => d.Id == staff);
        }
    }
}
