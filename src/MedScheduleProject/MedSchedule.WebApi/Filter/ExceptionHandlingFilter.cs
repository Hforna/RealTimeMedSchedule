using MedSchedule.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedSchedule.WebApi.Filter
{
    public class ExceptionHandlingFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is BaseException baseEx)
            {
                int statusCode = StatusCodes.Status400BadRequest;

                switch(context.Exception)
                {
                    case DomainException:
                        statusCode = StatusCodes.Status422UnprocessableEntity;
                        break;
                    case RequestException:
                        statusCode = StatusCodes.Status400BadRequest;
                        break;
                }

                context.HttpContext.Response.StatusCode = statusCode;
                context.Result = new JsonResult(baseEx);
            }else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new JsonResult(context.Exception);
            }
        }
    }
}
