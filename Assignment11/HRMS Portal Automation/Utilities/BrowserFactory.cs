using HRIntimeAutomation.Configuration;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Utilities;

public static class BrowserFactory
{
    public static Task<IBrowser> LaunchAsync(IPlaywright playwright, TestSettings settings)
    {
        var options = new BrowserTypeLaunchOptions
        {
            Headless = settings.Headless,
            Timeout = settings.TimeoutMilliseconds
        };

        if (settings.BrowserEngineType.Equals(BrowserEngineTypes.Chromium, StringComparison.OrdinalIgnoreCase))
            return playwright.Chromium.LaunchAsync(options);

        if (settings.BrowserEngineType.Equals(BrowserEngineTypes.WebKit, StringComparison.OrdinalIgnoreCase))
            return playwright.Webkit.LaunchAsync(options);

        if (settings.BrowserEngineType.Equals(BrowserEngineTypes.Firefox, StringComparison.OrdinalIgnoreCase))
            return playwright.Firefox.LaunchAsync(options);

        throw new InvalidOperationException(
            $"Unsupported browser engine type: {settings.BrowserEngineType}. Use Chromium, WebKit, or Firefox.");
    }
}
