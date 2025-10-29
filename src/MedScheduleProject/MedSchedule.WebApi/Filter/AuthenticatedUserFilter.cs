
using MedSchedule.Domain.Services;
using MedSchedule.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MedSchedule.Domain.Repositories;

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
        private readonly IUnitOfWork _uow;

        public AuthenticatedUserFilter(ITokenService tokenService, IRequestService requestService, IUnitOfWork uow)
        {
            _tokenService = tokenService;
            _requestService = requestService;
            _uow = uow;
        }
       
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = _requestService.GetBearerToken();

            if (string.IsNullOrEmpty(token))
                throw new NotAuthenticatedException("User must be authenticated to access this endpoint");

            try
            {
                var userId = _tokenService.GetUserGuidByToken();
                var user = await _uow.UserRepository.GetUserById((Guid)userId!);

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
