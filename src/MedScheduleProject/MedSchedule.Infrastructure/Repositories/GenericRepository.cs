using MedSchedule.Domain;
using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Repositories
{
    public class GenericRepository : IGenericRepository
    {
        private readonly ProjectDataContext _context;

        public GenericRepository(ProjectDataContext context)
        {
            _context = context;
        }

        public async Task Add<T>(T entity) where T : Entity
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void UpdateRange<T>(List<T> entities) where T : Entity
        {
            _context.Set<T>().UpdateRange(entities);
        }
    }
}
