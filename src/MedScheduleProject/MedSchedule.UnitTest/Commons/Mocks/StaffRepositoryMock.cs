using MedSchedule.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class StaffRepositoryMock
    {
        private readonly Mock<IStaffRepository> _mock = new Mock<IStaffRepository>();

        public IStaffRepository GetObject() => _mock.Object;
        public Mock<IStaffRepository> GetMock() => _mock;
    }
}
