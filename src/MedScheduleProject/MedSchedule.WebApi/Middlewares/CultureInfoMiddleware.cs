
using System.Globalization;

namespace MedSchedule.WebApi.Middlewares
{
    public class CultureInfoMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestCulture = context.Request.Headers.AcceptLanguage;
            var acceptedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var cultureInfo = new CultureInfo("en-US");

            if (string.IsNullOrEmpty(requestCulture) == false && acceptedCultures.Any(d => d.Equals(requestCulture)))
                cultureInfo = new CultureInfo(requestCulture!);

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            await next(context);
        }
    }
}
