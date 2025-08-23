
using MedSchedule.Domain.Services;
using MedSchedule.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MedSchedule.WebApi.Filter
{
    public class AuthenticatedUserAttribute : TypeFilterAttribute
    {
        public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
        {
        }
    }

    public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
    {
        private readonly ITokenService _tokenService;
        private readonly IRequestService _requestService;

        public AuthenticatedUserFilter(ITokenService tokenService, IRequestService requestService)
        {
            _tokenService = tokenService;
            _requestService = requestService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = _requestService.GetBearerToken();

            if (string.IsNullOrEmpty(token))
                throw new NotAuthenticatedException("User must be authenticated to access this endpoint");

            try
            {
                var user = await _tokenService.GetUserByToken();

                if (user is null)
                    throw new ResourceNotFoundException("User provided by token was not found");
            }
            catch (SecurityTokenExpiredException se)
            {
                throw new RequestException("Authentication token provided is expired");
            }
            catch (Exception ex)
            {
                throw new RequestException("It was not possible verify the authentication token provided");
            }
        }
    }
}
