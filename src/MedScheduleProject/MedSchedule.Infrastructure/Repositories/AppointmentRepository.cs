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

        public async Task<Staff?> GetStaffWithLessAppointments(List<Staff> staffs)
        {
            return await _context.Staffs
                .AsNoTracking()
                .Where(staff => staffs.Contains(staff))
                .OrderBy(d => d.ProfessionalInfos!.Appointments.Count)
                .FirstOrDefaultAsync();
        }
    }
}
