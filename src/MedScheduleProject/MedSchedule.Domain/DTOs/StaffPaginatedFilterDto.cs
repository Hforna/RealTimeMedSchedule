using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.DTOs
{
    public sealed record StaffPaginatedFilterDto(int perPage, int page, string? specialty, int? minTotalServices, int? maxTotalServices, WorkShiftDto? WorkShift)
    {

    }

    public sealed record WorkShiftDto(int startHours, int endHours, int startMinutes, int endMinutes)
    {

    }
}
