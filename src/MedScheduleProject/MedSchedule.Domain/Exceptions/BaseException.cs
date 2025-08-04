using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public abstract class BaseException : SystemException
    {
        public List<string> Errors { get; set; } = [];

        public BaseException(string message) : base(message)
        {
            Errors.Add(message);
        }

        public BaseException(List<string> errors) : base(string.Empty)
        {
            Errors = errors;
        }

        public List<string> GetMessages() => [Message];
    }
}
