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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MedSchedule.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ProjectDataContext _context;

        public AppointmentRepository(ProjectDataContext context)
        {
            _context = context;
        }

        public async Task<List<Staff>?> GetAllSpecialtyStaffAvaliableByIds(List<Guid> staffIds, DateTime time)
        {
            var staffs = _context.Staffs
                .Where(d => staffIds.Contains(d.Id))
                .AsQueryable();

            if (staffs is null)
                throw new ResourceNotFoundException("Id staffs was not found");

            var newHours = time.Hour;
            var newMinutes = time.Minute;
            var newDay = time.Day;

            var availableStaff = staffs.Include(d => d.Appointments)
                 .Where(s => s.WorkShift.IsTimeBetweenShift(time) && s.Appointments!.Any(a => a.Schedule.AppointmentDate == time 
                 && a.Schedule != null 
                 && time.TimeOfDay >= new TimeSpan(a.Schedule.StartHours, a.Schedule.StartMinutes, 0) 
                 && time.TimeOfDay <= new TimeSpan(a.Schedule.EndHours, a.Schedule.EndMinutes, 0)) == false); 

            return await availableStaff.ToListAsync();
        }
    }
}
