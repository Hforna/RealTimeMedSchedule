using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Exceptions;
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

        public async Task<List<Staff>?> GetAllSpecialtyStaffAvaliableByIds(List<Guid> staffIds, DateTime time)
        {
            var staffs = _context.Staffs
                .Where(d => staffIds.Contains(d.Id)
                    && d.Role == StaffRoles.Professional
                    && (time.Hour * 60 + time.Minute) >= (d.WorkShift.StartHours * 60 + d.WorkShift.StartMinutes)
                    && (time.Hour * 60 + time.Minute) <= (d.WorkShift.EndHours * 60 + d.WorkShift.EndMinutes));

            if (await staffs.AnyAsync() == false)
                throw new ResourceNotFoundException("Id staffs was not found");

            var newHours = time.Hour;
            var newMinutes = time.Minute;
            var newDay = time.Day;

            var timeOfDay = time.TimeOfDay;
            var timeInMinutes = timeOfDay.Hours * 60 + timeOfDay.Minutes;

            var availableStaff = staffs
                       .Where(s => s.ProfessionalInfos!.Appointments!
                       .Any(a => a.Schedule.AppointmentDate.Date == time.Date &&
                            (timeInMinutes >= a.Schedule.StartHours * 60 + a.Schedule.StartMinutes
                            && timeInMinutes <= a.Schedule.EndHours * 60 + a.Schedule.EndMinutes)) == false
                       );

            return await availableStaff.ToListAsync();
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
                .Where(d => d.ProfessionalInfos != null 
                && d.Role == StaffRoles.Professional 
                && d.ProfessionalInfos.SpecialtyId == specialtyId)
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
            return await _context.Staffs.Include(d => d.User).SingleOrDefaultAsync(d => d.Id == staff);
        }

        public async Task<ProfessionalInfos?> GetProfessionalInfosByStaffId(Guid staffId)
        {
            return await _context.ProfessionalsInfos.SingleOrDefaultAsync(d => d.StaffId == staffId);
        }

        public async Task<bool> UserStaffExists(Guid userId)
        {
            return await _context.Staffs.AnyAsync(d => d.UserId == userId);
        }
    }
}
