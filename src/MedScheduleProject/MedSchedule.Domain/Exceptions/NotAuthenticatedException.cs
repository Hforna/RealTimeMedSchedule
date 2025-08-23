using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public class UnauthenticatedException : BaseException
    {
        public UnauthenticatedException(string message) : base(message)
        {
        }

        public UnauthenticatedException(List<string> errors) : base(string.Empty)
        {
        }
    }

    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(List<string> errors) : base(string.Empty)
        {
        }
    }
}
