using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
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

        public async Task<Pagination<Appointment>> FilterAppointmentsPaginated(FilterAppointmentsDto dto)
        {
            var query = _context.Appointments.AsNoTracking();

            if (dto.staffId != null)
                query = query.Where(d => d.StaffId == dto.staffId);
            if(dto.queueDay is not null)
                query = query.Where(d => d.CheckInDate.Value.Date ==  dto.queueDay.Value.Date);
            if(!string.IsNullOrEmpty(dto.specialtyName))
                query = query.Where(d => d.Specialty.Name == dto.specialtyName.ToLower());
            if(dto.status is not null)
                query = query.Where(d => d.AppointmentStatus == dto.status);
            if(dto.priorityLevel is not null)
                query = query.Where(d => d.PriorityLevel == dto.priorityLevel);
            if(dto.patientId is not null)
                query = query.Where(d => d.PatientId == dto.patientId);

            return await query
                .OrderByDescending(d => d.CreatedAt)
                .AsPaginationAsync(dto.page, dto.perPage);
        }

        public async Task<Staff?> GetStaffWithLessAppointments(List<Staff> staffs)
        {
            return await _context.Staffs
                .AsNoTracking()
                .Include(d => d.User)
                .Where(staff => staffs.Contains(staff))
                .OrderBy(d => d.ProfessionalInfos!.Appointments.Count)
                .FirstOrDefaultAsync();
        }
    }
}
