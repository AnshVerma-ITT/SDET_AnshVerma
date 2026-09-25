using System.Globalization;
using System.Text.Json;
using NUnit.Framework;

namespace HRIntimeAutomation.Configuration;

public sealed class TestSettings
{
    private const string SettingsFileName = "appsettings.json";

    private const string BaseUrlVariable = "HRMS_BASE_URL";
    private const string BrowserEngineTypeVariable = "BROWSER_ENGINE_TYPE";
    private const string LegacyBrowserVariable = "BROWSER";
    private const string HeadlessVariable = "HEADLESS";
    private const string TimeoutVariable = "TIMEOUT_MILLISECONDS";
    private const string ResponseTimeoutVariable = "RESPONSE_TIMEOUT_MILLISECONDS";
    private const string ArtifactsDirectoryVariable = "HRMS_ARTIFACTS_DIR";

    public required string BaseUrl { get; init; }
    public required string BrowserEngineType { get; init; }
    public bool Headless { get; init; }
    public int TimeoutMilliseconds { get; init; }
    public int ResponseTimeoutMilliseconds { get; init; }
    public int ViewportWidth { get; init; }
    public int ViewportHeight { get; init; }
    public bool IgnoreHttpsErrors { get; init; }
    public bool TraceOnFailure { get; init; }
    public bool ScreenshotOnFailure { get; init; }
    public required string ArtifactsDirectory { get; init; }
    public required string TestIdAttribute { get; init; }
    public string ApplicationTimeZoneId { get; init; } = string.Empty;

    public static TestSettings Load()
    {
        var filePath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            SettingsFileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Could not find {SettingsFileName} in the test output directory.",
                filePath);
        }

        var settings = JsonSerializer.Deserialize<TestSettings>(
            File.ReadAllText(filePath),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
            ?? throw new InvalidOperationException(
                $"{SettingsFileName} is invalid.");

        settings = settings.WithEnvironmentOverrides();
        settings.Validate();
        return settings;
    }

    public string GetArtifactsDirectory()
    {
        var configuredPath = GetString(
            ArtifactsDirectoryVariable,
            ArtifactsDirectory);

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                configuredPath);
    }

    private TestSettings WithEnvironmentOverrides()
    {
        return new TestSettings
        {
            BaseUrl = GetString(
                BaseUrlVariable,
                BaseUrl),

            BrowserEngineType = GetBrowserEngineType(BrowserEngineType),

            Headless = GetBoolean(
                HeadlessVariable,
                Headless),

            TimeoutMilliseconds = GetInteger(
                TimeoutVariable,
                TimeoutMilliseconds),

            ResponseTimeoutMilliseconds = GetInteger(
                ResponseTimeoutVariable,
                ResponseTimeoutMilliseconds),

            ViewportWidth = ViewportWidth,

            ViewportHeight = ViewportHeight,

            IgnoreHttpsErrors = IgnoreHttpsErrors,

            TraceOnFailure = TraceOnFailure,

            ScreenshotOnFailure = ScreenshotOnFailure,

            ArtifactsDirectory = GetString(
                ArtifactsDirectoryVariable,
                ArtifactsDirectory),

            TestIdAttribute = TestIdAttribute,

            ApplicationTimeZoneId = ApplicationTimeZoneId
        };
    }

    private void Validate()
    {
        if (!Uri.TryCreate(
                BaseUrl,
                UriKind.Absolute,
                out var baseUri)
            || (baseUri.Scheme != Uri.UriSchemeHttp
                && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                "baseUrl/HRMS_BASE_URL must be an absolute HTTP or HTTPS URL.");
        }

        if (!BrowserEngineTypes.IsSupported(BrowserEngineType))
        {
            throw new InvalidOperationException(
                $"Unsupported browser engine type '{BrowserEngineType}'. " +
                "Use Chromium or WebKit.");
        }

        if (TimeoutMilliseconds <= 0
            || ResponseTimeoutMilliseconds <= 0
            || ViewportWidth <= 0
            || ViewportHeight <= 0)
        {
            throw new InvalidOperationException(
                "Timeouts and viewport dimensions must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(ArtifactsDirectory)
            || string.IsNullOrWhiteSpace(TestIdAttribute))
        {
            throw new InvalidOperationException(
                "artifactsDirectory and testIdAttribute are required.");
        }
    }

    private static string GetBrowserEngineType(string fallback)
    {
        var explicitEngine = Environment.GetEnvironmentVariable(BrowserEngineTypeVariable);
        if (!string.IsNullOrWhiteSpace(explicitEngine))
            return explicitEngine.Trim();

        // Backward compatibility for CI jobs that expose the Jenkins BROWSER parameter.
        // Ignore values such as "All" because they are orchestration choices, not engines.
        var legacyBrowser = Environment.GetEnvironmentVariable(LegacyBrowserVariable);
        if (!string.IsNullOrWhiteSpace(legacyBrowser)
            && BrowserEngineTypes.IsSupported(legacyBrowser.Trim()))
        {
            return legacyBrowser.Trim();
        }

        return fallback;
    }

    private static string GetString(
        string variable,
        string fallback)
    {
        var value = Environment.GetEnvironmentVariable(variable);

        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim();
    }

    private static bool GetBoolean(
        string variable,
        bool fallback)
    {
        var value = Environment.GetEnvironmentVariable(variable);

        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : bool.Parse(value);
    }

    private static int GetInteger(
        string variable,
        int fallback)
    {
        var value = Environment.GetEnvironmentVariable(variable);

        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : int.Parse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture);
    }
}