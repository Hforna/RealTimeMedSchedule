using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ProjectDataContext _context;

        public AppointmentRepository(ProjectDataContext context)
        {
            _context = context;
        }


    }
}
