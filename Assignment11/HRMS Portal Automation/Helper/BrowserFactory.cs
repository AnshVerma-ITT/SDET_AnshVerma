using HRIntimeAutomation.Configuration;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Helper;

public static class BrowserFactory
{
    private const string ChromeChannel = "chrome";
    private const string EdgeChannel = "msedge";

    public static Task<IBrowser> LaunchAsync(IPlaywright playwright, TestSettings settings)
    {
        var options = new BrowserTypeLaunchOptions { Headless = settings.Headless };

        if (settings.Browser.Equals(BrowserNames.Chrome, StringComparison.OrdinalIgnoreCase))
        {
            options.Channel = ChromeChannel;
            return playwright.Chromium.LaunchAsync(options);
        }

        if (settings.Browser.Equals(BrowserNames.Edge, StringComparison.OrdinalIgnoreCase))
        {
            options.Channel = EdgeChannel;
            return playwright.Chromium.LaunchAsync(options);
        }

        if (settings.Browser.Equals(BrowserNames.Chromium, StringComparison.OrdinalIgnoreCase))
            return playwright.Chromium.LaunchAsync(options);
        if (settings.Browser.Equals(BrowserNames.Firefox, StringComparison.OrdinalIgnoreCase))
            return playwright.Firefox.LaunchAsync(options);
        if (settings.Browser.Equals(BrowserNames.WebKit, StringComparison.OrdinalIgnoreCase))
            return playwright.Webkit.LaunchAsync(options);

        throw new InvalidOperationException($"Unsupported browser: {settings.Browser}");
    }
}
