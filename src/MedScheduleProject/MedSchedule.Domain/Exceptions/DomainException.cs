using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public class DomainException : SystemException
    {
        public List<string> Errors { get; set; } = [];

        public DomainException(string message) : base(message)
        {
            Errors.Add(message);
        }

        public DomainException(List<string> errors) : base(string.Empty)
        {
            Errors = errors;
        }

        public List<string> GetMessages() => [Message];
    }
}
