using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace BaSys.Host.Extensions
{
    public static class LocalizationExtension
    {
        public static IApplicationBuilder UseCookieRequestLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ru-RU"),
            };

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            requestLocalizationOptions.RequestCultureProviders = new[] { new CookieRequestCultureProvider() };

            app.UseRequestLocalization(requestLocalizationOptions);

            return app;
        }
    }
}
