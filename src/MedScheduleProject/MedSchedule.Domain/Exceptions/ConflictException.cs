using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public class ConflictException : BaseException
    {
        public ConflictException(List<string> errors) : base(errors)
        {
        }

        public ConflictException(string message) : base(message)
        {

        }
    }
}
