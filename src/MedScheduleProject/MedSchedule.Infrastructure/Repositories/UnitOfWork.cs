using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectDataContext _context;

        public UnitOfWork(ProjectDataContext context, IUserRepository userRepository, 
            IAppointmentRepository appointmentRepository, IGenericRepository genericRepository, 
            IQueueRepository queueRepository, IStaffRepository staffRepository)
        {
            _context = context;
            UserRepository = userRepository;
            AppointmentRepository = appointmentRepository;
            GenericRepository = genericRepository;
            QueueRepository = queueRepository;
            StaffRepository = staffRepository;
        }

        public IUserRepository UserRepository { get; }
        public IAppointmentRepository AppointmentRepository { get; }
        public IGenericRepository GenericRepository { get; }
        public IQueueRepository QueueRepository { get; }
        public IStaffRepository StaffRepository { get; }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }
    }
}
