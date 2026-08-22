using System.Text.Json;
using NUnit.Framework;

namespace SauceDemo.Playwright.Tests.Configuration;

public sealed class TestSettings
{
    public string BaseUrl { get; set; } = "https://www.saucedemo.com";
    public bool Headless { get; set; } = true;
    public int TimeoutMilliseconds { get; set; } = 30000;
    public bool ScreenshotOnFailure { get; set; } = true;
    public bool TraceEnabled { get; set; } = true;
    public bool UseChromeChannel { get; set; }

    public static TestSettings Load()
    {
        var settings = new TestSettings();
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "appsettings.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            settings = JsonSerializer.Deserialize<TestSettings>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? settings;
        }

        return settings;
    }
}
