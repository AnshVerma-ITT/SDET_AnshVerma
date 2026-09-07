using System.Text.Json;
using NUnit.Framework;

namespace SauceDemo.Playwright.Tests.Configuration;

public sealed class TestSettings
{
    public required string BaseUrl { get; set; }
    public bool Headless { get; set; }
    public int TimeoutMilliseconds { get; set; }
    public int NavigationMaxAttempts { get; set; }
    public int NavigationRetryDelayMilliseconds { get; set; }
    public int RandomSeed { get; set; }
    public bool ScreenshotOnFailure { get; set; }
    public bool TraceEnabled { get; set; }
    public bool VideoEnabled { get; set; }

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

        if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("appsettings.json baseUrl must be an absolute HTTP or HTTPS URL.");
        }

        if (settings.TimeoutMilliseconds <= 0)
        {
            throw new InvalidOperationException("appsettings.json timeoutMilliseconds must be greater than zero.");
        }

        if (settings.NavigationMaxAttempts <= 0)
        {
            throw new InvalidOperationException("appsettings.json navigationMaxAttempts must be greater than zero.");
        }

        if (settings.NavigationRetryDelayMilliseconds < 0)
        {
            throw new InvalidOperationException(
                "appsettings.json navigationRetryDelayMilliseconds cannot be negative.");
        }

        return settings;
    }
}
