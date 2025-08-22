using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Persistence;
using MedSchedule.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class UnitOfWorkMock
    {

        public IUnitOfWork Create(IUserRepository? userRepository = null,
            IAppointmentRepository? appointmentRepository = null, IGenericRepository? genericRepository = null, IQueueRepository? queueRepository = null)
        {
            userRepository = userRepository ?? new Mock<IUserRepository>().Object;
            appointmentRepository = appointmentRepository ?? new Mock<IAppointmentRepository>().Object;
            genericRepository = genericRepository ?? new Mock<IGenericRepository>().Object;
            queueRepository = queueRepository ?? new Mock<IQueueRepository>().Object;
            var dbContextMock = new Mock<ProjectDataContext>(new DbContextOptions<ProjectDataContext>());

            return new UnitOfWork(dbContextMock.Object, userRepository, 
                appointmentRepository, genericRepository, queueRepository);
        }
    }
}
