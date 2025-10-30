using MedSchedule.Domain.AggregatesModel.AppointmentAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.DTOs;
using Pagination.EntityFrameworkCore.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Staff?> GetStaffWithLessAppointments(List<Staff> staffs);
        Task<Pagination<Appointment>> FilterAppointmentsPaginated(FilterAppointmentsDto dto);
    }
}
