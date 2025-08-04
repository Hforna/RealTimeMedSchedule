using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Services
{
    public interface IPasswordEncryptService
    {
        public string GenerateHash(string password);
        public bool Verify(string password, string hash);
    }
}
