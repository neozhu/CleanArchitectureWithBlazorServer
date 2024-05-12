using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using IMiddleware = Microsoft.AspNetCore.Http.IMiddleware;

namespace CleanArchitecture.Blazor.Server.UI.Middlewares;
#nullable disable
public class LocalizationCookiesMiddleware : IMiddleware
{
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

    public CookieRequestCultureProvider Provider { get; }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (Provider != null)
        {
            var feature = context.Features.Get<IRequestCultureFeature>();

            if (feature != null)
                // remember culture across request
                context.Response
                    .Cookies
                    .Append(
                        Provider.CookieName,
                        CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture),
                        new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddMonths(3)) }
                    );
        }

        await next(context);
    }
}