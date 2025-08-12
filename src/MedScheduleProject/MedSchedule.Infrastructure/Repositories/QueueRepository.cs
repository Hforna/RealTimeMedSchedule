using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ProjectDataContext _context;

        public QueueRepository(ProjectDataContext context)
        {
            _context = context;
        }

        public async Task<QueueRoot?> GetQueueRoot(Guid specialtyId, DateTime time)
        {
            return await _context.QueueRoots.FirstOrDefaultAsync(d => d.QueueDate == time && d.SpecialtyId == specialtyId);
        }
    }
}
