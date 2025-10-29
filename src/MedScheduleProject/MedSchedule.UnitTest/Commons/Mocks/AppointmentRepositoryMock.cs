using MedSchedule.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class AppointmentRepositoryMock
    {
        private readonly Mock<IAppointmentRepository> _mock = new Mock<IAppointmentRepository>();

        public IAppointmentRepository GetObject() => _mock.Object;
        public Mock<IAppointmentRepository> GetMock() => _mock;
    }
}
