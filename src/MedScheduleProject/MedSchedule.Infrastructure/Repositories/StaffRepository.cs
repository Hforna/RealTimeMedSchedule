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
    public class StaffRepository : IStaffRepository
    {
        private readonly ProjectDataContext _context;

        public StaffRepository(ProjectDataContext context)
        {
            _context = context;
        }

        public async Task<Staff?> GetStaffByUserId(Guid userId)
        {
            return await _context.Staffs
                .Include(d => d.User)
                .Include(d => d.ProfessionalInfos)
                .ThenInclude(d => d.Specialty)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
