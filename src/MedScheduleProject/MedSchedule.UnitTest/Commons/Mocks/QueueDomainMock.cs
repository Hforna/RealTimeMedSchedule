using Bogus;
using MedSchedule.Domain.AggregatesModel.QueueAggregate;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class QueueDomainMock
    {
        private readonly Mock<IQueueDomainService> _mock = new Mock<IQueueDomainService>();

        public IQueueDomainService GetObject() => _mock.Object;
        public Mock<IQueueDomainService> GetMock() => _mock;
    }
}
