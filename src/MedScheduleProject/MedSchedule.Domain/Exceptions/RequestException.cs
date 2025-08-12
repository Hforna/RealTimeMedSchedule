using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public class RequestException : BaseException
    {
        public RequestException(List<string> errors) : base(errors)
        {
        }

        public RequestException(string message) : base(message)
        {

        }
    }

    public class ResourceNotFoundException : BaseException
    {
        public ResourceNotFoundException(List<string> errors) : base(errors)
        {
        }

        public ResourceNotFoundException(string message) : base(message)
        {

        }
    }
}
