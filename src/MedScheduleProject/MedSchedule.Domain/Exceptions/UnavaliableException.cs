﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Exceptions
{
    public class UnavaliableException : BaseException
    {
        public UnavaliableException(string message) : base(message)
        {
        }

        public UnavaliableException(List<string> errors) : base(string.Empty)
        {
        }
    }
}
