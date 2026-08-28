using System.Text.Json;
using NUnit.Framework;

namespace PlaywrightAdvancedFeatures.Configuration;

public sealed class TestSettings
{
    public required string Browser { get; init; }
    public bool Headless { get; init; }
    public int TimeoutMilliseconds { get; init; }
    public bool TraceEnabled { get; init; }
    public bool VideoEnabled { get; init; }
    public bool ScreenshotOnFailure { get; init; }
    public required string EvidenceDirectory { get; init; }

    public static TestSettings Load()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "appsettings.json");
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TestSettings>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("appsettings.json is invalid.");
    }
}
