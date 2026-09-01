using Microsoft.Playwright;
using SauceDemo.Playwright.Tests.Configuration;

namespace SauceDemo.Playwright.Tests.Fixtures;

public static class BrowserFactory
{
    public static Task<IBrowser> Launch(
        IPlaywright playwright,
        BrowserEngine browserEngine,
        TestSettings settings)
    {
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = settings.Headless
        };

        return browserEngine switch
        {
            BrowserEngine.Chromium => playwright.Chromium.LaunchAsync(launchOptions),
            BrowserEngine.WebKit => playwright.Webkit.LaunchAsync(launchOptions),
            _ => throw new NotSupportedException($"Unsupported browser engine: {browserEngine}")
        };
    }
}
