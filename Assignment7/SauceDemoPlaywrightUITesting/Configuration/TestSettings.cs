using System.Text.Json;
using NUnit.Framework;

namespace SauceDemo.Playwright.Tests.Configuration;

public sealed class TestSettings
{
    public required string BaseUrl { get; set; }
    public bool Headless { get; set; }
    public int TimeoutMilliseconds { get; set; }
    public bool ScreenshotOnFailure { get; set; }
    public bool TraceEnabled { get; set; }
    public bool UseChromeChannel { get; set; }

    public static TestSettings Load()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "appsettings.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Test configuration file was not found.", path);
        }

        var json = File.ReadAllText(path);
        var settings = JsonSerializer.Deserialize<TestSettings>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("appsettings.json does not contain valid test settings.");

        if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("appsettings.json must contain a valid absolute baseUrl.");
        }

        if (settings.TimeoutMilliseconds <= 0)
        {
            throw new InvalidOperationException("appsettings.json timeoutMilliseconds must be greater than zero.");
        }

        return settings;
    }
}
