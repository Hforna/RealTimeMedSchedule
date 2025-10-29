using MedSchedule.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class UnitOfWorkMock
    {
        private readonly Mock<IUnitOfWork> _mock;

        public UnitOfWorkMock()
        {
            _mock = new Mock<IUnitOfWork>();
        }

        public IUnitOfWork Create(IUserRepository? userRepository = null,
            IAppointmentRepository? appointmentRepository = null,
            IGenericRepository? genericRepository = null,
            IQueueRepository? queueRepository = null)
        {

            userRepository ??= new Mock<IUserRepository>().Object;
            appointmentRepository ??= new Mock<IAppointmentRepository>().Object;
            genericRepository ??= new Mock<IGenericRepository>().Object;
            queueRepository ??= new Mock<IQueueRepository>().Object;


            _mock.Setup(u => u.UserRepository).Returns(userRepository);
            _mock.Setup(u => u.AppointmentRepository).Returns(appointmentRepository);
            _mock.Setup(u => u.GenericRepository).Returns(genericRepository);
            _mock.Setup(u => u.QueueRepository).Returns(queueRepository);


            _mock.Setup(u => u.Commit()).Returns(Task.CompletedTask);


            var transactionMock = new Mock<IDbContextTransaction>();
            //transactionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
            //transactionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);
            transactionMock.Setup(t => t.Dispose()).Verifiable();

            _mock.Setup(u => u.BeginTransaction()).ReturnsAsync(transactionMock.Object);

            return _mock.Object;
        }

        // Método adicional para obter o mock e fazer verificações
        public Mock<IUnitOfWork> GetMock()
        {
            return _mock;
        }
    }
}