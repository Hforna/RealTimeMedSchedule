using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Services
{
    public interface IRequestService
    {
        public string GetBearerToken();
    }
}
