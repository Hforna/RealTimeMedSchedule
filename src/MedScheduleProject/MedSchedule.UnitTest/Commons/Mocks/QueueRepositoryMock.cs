using MedSchedule.Domain.Repositories;
using MedSchedule.Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class QueueRepositoryMock
    {
        private readonly Mock<IQueueRepository> _mock = new Mock<IQueueRepository>();

        public IQueueRepository GetObject() => _mock.Object;
        public Mock<IQueueRepository> GetMock() => _mock;
    }
}
