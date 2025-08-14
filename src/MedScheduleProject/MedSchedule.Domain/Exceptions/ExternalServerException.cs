using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public class ExternalServerException : BaseException
    {
        public ExternalServerException(List<string> errors) : base(errors)
        {
        }

        public ExternalServerException(string message) : base(message)
        {

        }
    }

    public class InternalServerException : BaseException
    {
        public InternalServerException(List<string> errors) : base(errors)
        {
        }

        public InternalServerException(string message) : base(message)
        {

        }
    }
}
