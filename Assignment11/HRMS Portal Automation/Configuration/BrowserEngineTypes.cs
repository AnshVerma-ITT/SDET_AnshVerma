namespace HRIntimeAutomation.Configuration;

public static class BrowserEngineTypes
{
    public const string Chromium = "Chromium";
    public const string WebKit = "WebKit";
    public const string Firefox = "Firefox";

    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        Chromium,
        WebKit,
        Firefox
    };

    public static bool IsSupported(string browserEngineType) => Supported.Contains(browserEngineType);
}
