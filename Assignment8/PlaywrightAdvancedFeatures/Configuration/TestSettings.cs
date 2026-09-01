using System.Text.Json;
using NUnit.Framework;

namespace PlaywrightAdvancedFeatures.Configuration;

public sealed class TestSettings
{
    private const string SettingsFileName = "appsettings.json";

    public required string Browser { get; init; }
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
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, SettingsFileName);
        var json = File.ReadAllText(path);
        var settings = JsonSerializer.Deserialize<TestSettings>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException($"{SettingsFileName} is invalid.");

        if (!BrowserConfiguration.IsSupported(settings.Browser))
        {
            throw new InvalidOperationException(BrowserConfiguration.ValidationMessage);
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

        return settings;
    }
}
