using MedSchedule.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedSchedule.WebApi.Filter
{
    public class ExceptionHandlingFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BaseException baseEx)
            {
                int statusCode = StatusCodes.Status400BadRequest;

                switch (context.Exception)
                {
                    case DomainException:
                        statusCode = StatusCodes.Status422UnprocessableEntity;
                        break;
                    case RequestException:
                        statusCode = StatusCodes.Status400BadRequest;
                        break;
                    case ResourceNotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        break;
                    case ConflictException:
                        statusCode = StatusCodes.Status409Conflict;
                        break;
                    case NotAuthenticatedException:
                        statusCode = StatusCodes.Status400BadRequest;
                        break;
                    case UnavaliableException:
                        statusCode = StatusCodes.Status503ServiceUnavailable;
                        break;
                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        break;
                    case UnauthorizedException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        break;
                }

                context.HttpContext.Response.StatusCode = statusCode;
                context.Result = new ObjectResult(new {
                    Messages = baseEx.GetMessages(),
                    ErrorCode = statusCode
                });
            }
            else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new ObjectResult(new
                {
                    Message = context.Exception.Message,
                    ErrorCode = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
