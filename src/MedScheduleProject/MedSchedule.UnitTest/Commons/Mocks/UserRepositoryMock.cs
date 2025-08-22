using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class UserRepositoryMock
    {
        private Mock<IUserRepository> _mock = new Mock<IUserRepository>();

        public IUserRepository GetObject() => _mock.Object;
        public Mock<IUserRepository> GetMock() => _mock;
        public void SetUserById(User? user)
        {
            _mock.Setup(d => d.GetUserById(It.IsAny<Guid>())).ReturnsAsync(user);
        }
    }
}
