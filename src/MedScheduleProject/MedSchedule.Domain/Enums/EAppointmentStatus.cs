using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Enums
{
    public enum EAppointmentStatus
    {
        Scheduled,
        CheckedIn,
        InProgress,
        Completed,
        Bumped,
        Cancelled
    }
}
