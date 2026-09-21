namespace HRIntimeAutomation.Configuration;

public static class BrowserEngineTypes
{
    public const string Chrome = "Chrome";
    public const string Chromium = "Chromium";
    public const string Firefox = "Firefox";
    public const string WebKit = "WebKit";
    public const string Edge = "Edge";

    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        Chrome,
        Chromium,
        Firefox,
        WebKit,
        Edge
    };

    public static bool IsSupported(string browserEngineType) => Supported.Contains(browserEngineType);
}
