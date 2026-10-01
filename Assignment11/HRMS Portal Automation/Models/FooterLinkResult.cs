namespace HRIntimeAutomation.Models;

public sealed record FooterLinkResult(
    string Name,
    int MatchingLinkCount,
    bool IsVisible,
    string? Href,
    string? Target,
    string? OpenedUrl,
    string? OpenedTitle);
