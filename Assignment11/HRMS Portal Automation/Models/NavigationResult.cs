namespace HRIntimeAutomation.Models;

public sealed record NavigationResult(
    string Destination,
    string ExpectedRoute,
    string CurrentUrl,
    string PageTitle,
    string DestinationMarkerText,
    bool DestinationIsVisible,
    bool DestinationIsActive);
