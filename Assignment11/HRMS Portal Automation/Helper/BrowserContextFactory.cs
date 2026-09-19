using HRIntimeAutomation.Configuration;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Helper;

public static class BrowserContextFactory
{
    public static async Task<IBrowserContext> CreateAsync(IBrowser browser, TestSettings settings)
    {
        var options = new BrowserNewContextOptions
        {
            BaseURL = settings.BaseUrl,
            IgnoreHTTPSErrors = settings.IgnoreHttpsErrors,
            ViewportSize = new ViewportSize
            {
                Width = settings.ViewportWidth,
                Height = settings.ViewportHeight
            }
        };

        if (!string.IsNullOrWhiteSpace(settings.ApplicationTimeZoneId))
            options.TimezoneId = settings.ApplicationTimeZoneId;

        var context = await browser.NewContextAsync(options);
        context.SetDefaultTimeout(settings.TimeoutMilliseconds);
        context.SetDefaultNavigationTimeout(settings.TimeoutMilliseconds);
        return context;
    }
}
