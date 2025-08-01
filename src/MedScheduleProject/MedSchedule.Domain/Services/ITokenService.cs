﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Services
{
    public interface ITokenService
    {
        public string GenerateToken(List<Claim> claims, Guid userId);
    }
}
