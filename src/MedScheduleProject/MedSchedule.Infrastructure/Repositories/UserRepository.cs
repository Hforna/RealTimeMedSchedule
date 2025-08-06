using MedSchedule.Domain.Aggregates.UserAggregate;
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
    public class UserRepository : IUserRepository
    {
        private readonly ProjectDataContext _context;

        public UserRepository(ProjectDataContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserById(Guid id) => await _context.Users.SingleOrDefaultAsync(d => d.Id == id);
    }
}
