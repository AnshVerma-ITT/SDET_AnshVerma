using Microsoft.Playwright;
using PlaywrightAdvancedFeatures.Configuration;

namespace PlaywrightAdvancedFeatures.Fixtures;

public static class BrowserFactory
{
    public static Task<IBrowser> Launch(IPlaywright playwright, TestSettings settings)
    {
        var browserName = Environment.GetEnvironmentVariable(BrowserConfiguration.EnvironmentVariable)
            ?? settings.Browser;
        var options = new BrowserTypeLaunchOptions { Headless = settings.Headless };

        if (browserName.Equals(BrowserConfiguration.Chromium, StringComparison.OrdinalIgnoreCase))
        {
            return playwright.Chromium.LaunchAsync(options);
        }

        if (browserName.Equals(BrowserConfiguration.WebKit, StringComparison.OrdinalIgnoreCase))
        {
            return playwright.Webkit.LaunchAsync(options);
        }

        throw new ArgumentException(BrowserConfiguration.ValidationMessage);
    }
}
