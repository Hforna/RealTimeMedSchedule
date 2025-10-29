using MedSchedule.Domain.Services;
using MedSchedule.Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.UnitTest.Commons.Mocks
{
    public class TokenServiceMock
    {
        private readonly Mock<ITokenService> _mock = new Mock<ITokenService>();

        public ITokenService GetObject() => _mock.Object;

        public Mock<ITokenService> GetMock() => _mock;
    }
}
