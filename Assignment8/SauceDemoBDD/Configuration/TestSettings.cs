using System.Text.Json;
using NUnit.Framework;

namespace SauceDemoBDD.Configuration;

public sealed class TestSettings
{
    private const string SettingsFileName = "appsettings.json";

    public required string BaseUrl { get; init; }
    public required string Browser { get; init; }
    public required string TestIdAttribute { get; init; }
    public bool Headless { get; init; }
    public int TimeoutMilliseconds { get; init; }
    public int ViewportWidth { get; init; }
    public int ViewportHeight { get; init; }
    public bool TraceEnabled { get; init; }
    public bool VideoEnabled { get; init; }
    public bool ScreenshotOnFailure { get; init; }
    public required string EvidenceDirectory { get; init; }

    public static TestSettings Load()
    {
        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, SettingsFileName);
        var json = File.ReadAllText(filePath);

        var settings = JsonSerializer.Deserialize<TestSettings>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException($"{SettingsFileName} is invalid.");

        if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var baseUri)
            || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("baseUrl must be an absolute HTTP or HTTPS URL.");
        }

        if (!string.Equals(settings.Browser, BrowserNames.Chromium, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(settings.Browser, BrowserNames.WebKit, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("browser must be Chromium or WebKit.");
        }

        if (settings.TimeoutMilliseconds <= 0)
        {
            throw new InvalidOperationException("timeoutMilliseconds must be greater than zero.");
        }

        if (settings.ViewportWidth <= 0 || settings.ViewportHeight <= 0)
        {
            throw new InvalidOperationException("viewportWidth and viewportHeight must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(settings.EvidenceDirectory))
        {
            throw new InvalidOperationException("evidenceDirectory is required.");
        }

        if (string.IsNullOrWhiteSpace(settings.TestIdAttribute))
        {
            throw new InvalidOperationException("testIdAttribute is required.");
        }

        return settings;
    }
}
