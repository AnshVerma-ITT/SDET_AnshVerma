namespace SauceDemo.Playwright.Tests.Configuration;

public static class BrowserMatrix
{
    public static IEnumerable<BrowserEngine> Engines()
    {
        var browser = Environment.GetEnvironmentVariable("BROWSER");

        if (string.IsNullOrWhiteSpace(browser) ||
            browser.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            yield return BrowserEngine.Chromium;
            yield return BrowserEngine.WebKit;
            yield break;
        }

        foreach (var value in browser.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Enum.TryParse<BrowserEngine>(value, ignoreCase: true, out var engine))
            {
                yield return engine;
                continue;
            }

            throw new ArgumentException($"Unsupported BROWSER value '{value}'. Use Chromium, WebKit, or All.");
        }
    }
}
