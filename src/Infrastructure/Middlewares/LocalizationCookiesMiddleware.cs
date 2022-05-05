using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace CleanArchitecture.Blazor.Infrastructure.Middlewares;

public class LocalizationCookiesMiddleware : IMiddleware
{
    public CookieRequestCultureProvider Provider { get; }

    public LocalizationCookiesMiddleware(IOptions<RequestLocalizationOptions> requestLocalizationOptions)
    {
        Provider =
            requestLocalizationOptions
                .Value
                .RequestCultureProviders
                .Where(x => x is CookieRequestCultureProvider)
                .Cast<CookieRequestCultureProvider>()
                .FirstOrDefault();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (Provider != null)
        {
            var feature = context.Features.Get<IRequestCultureFeature>();

            if (feature != null)
            {
                // remember culture across request
                context.Response
                    .Cookies
                    .Append(
                        Provider.CookieName,
                        CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture),
                        new CookieOptions() { Expires = new DateTimeOffset(DateTime.Now.AddMonths(3)) }
                    );
            }
        }

        await next(context);
    }
}