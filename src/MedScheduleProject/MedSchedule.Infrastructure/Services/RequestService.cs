using MedSchedule.Domain.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Services
{
    public class RequestService : IRequestService
    {
        private readonly IHttpContextAccessor _httpContext;

        public RequestService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetBearerToken()
        {
            var token = _httpContext.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                return "";

            return token["Bearer ".Length..].Trim();
        }
    }
}
