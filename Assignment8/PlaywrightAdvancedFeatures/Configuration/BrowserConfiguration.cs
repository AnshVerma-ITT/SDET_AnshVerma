namespace PlaywrightAdvancedFeatures.Configuration;

public static class BrowserConfiguration
{
    public const string EnvironmentVariable = "BROWSER";
    public const string Chromium = "Chromium";
    public const string WebKit = "WebKit";
    public const string ValidationMessage = "Browser must be Chromium or WebKit.";

    public static bool IsSupported(string? browserName)
    {
        return string.Equals(browserName, Chromium, StringComparison.OrdinalIgnoreCase)
            || string.Equals(browserName, WebKit, StringComparison.OrdinalIgnoreCase);
    }
}
