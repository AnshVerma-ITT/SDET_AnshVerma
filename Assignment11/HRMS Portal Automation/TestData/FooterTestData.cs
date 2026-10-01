namespace HRIntimeAutomation.TestData;

public sealed record SocialLinkData(string Name, string Href, IReadOnlyList<string> AllowedHosts);

public static class FooterTestData
{
    public const string NewWindowTarget = "_blank";

    public static readonly IReadOnlyList<string> BrowserErrorUrlPrefixes =
        ["about:", "chrome-error:", "edge-error:"];

    public static readonly IReadOnlyList<string> ErrorIndicators =
        ["404", "not found", "page unavailable", "application error"];

    public static readonly IReadOnlyList<SocialLinkData> SocialLinks =
    [
        new("YouTube", "https://www.youtube.com/c/InTimeTecCreatingAbundance", ["youtube.com", "www.youtube.com"]),
        new("Facebook", "https://www.facebook.com/InTimeTec/", ["facebook.com", "www.facebook.com"]),
        new("LinkedIn", "https://in.linkedin.com/company/in-time-tec", ["linkedin.com", "in.linkedin.com", "www.linkedin.com"]),
        new("X", "https://twitter.com/intime_tec", ["twitter.com", "www.twitter.com", "x.com", "www.x.com"])
    ];
}
