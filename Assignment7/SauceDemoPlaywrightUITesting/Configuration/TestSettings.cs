using System.Text.Json;
using NUnit.Framework;

namespace SauceDemo.Playwright.Tests.Configuration;

public sealed class TestSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public bool Headless { get; set; } 
    public int TimeoutMilliseconds { get; set; } 
    public bool ScreenshotOnFailure { get; set; } 
    public bool TraceEnabled { get; set; } 
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
