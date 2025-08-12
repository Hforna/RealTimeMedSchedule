using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IAppointmentRepository AppointmentRepository { get; }
        public IGenericRepository GenericRepository { get; }
        public IQueueRepository  QueueRepository { get; }
        public Task Commit();
    }
}
