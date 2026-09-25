namespace HRIntimeAutomation.Models;

public sealed record LoginValidationResult(
    bool LoginButtonEnabled,
    bool UsernameValid,
    bool PasswordValid,
    string UsernameValidationMessage,
    string PasswordValidationMessage,
    IReadOnlyList<string> VisibleValidationMessages,
    bool LoginFormVisible,
    bool DashboardVisible,
    string CurrentUrl);

public sealed record SessionSecurityResult(
    bool DirectAccessShowsLogin,
    string DirectAccessUrl,
    bool RefreshShowsLogin,
    string RefreshUrl,
    bool BackNavigationShowsLogin,
    string BackNavigationUrl);

public sealed record FormValidationResult(
    string FormName,
    string OmittedField,
    bool SubmitButtonEnabled,
    bool DialogVisible,
    IReadOnlyList<string> ValidationMessages,
    string? NotificationText);

public sealed record AttendanceRangeSelection(
    DateTime StartDate,
    DateTime EndDate,
    IReadOnlyList<DateTime> ExpectedDates);

public sealed record FooterLinkResult(
    string Name,
    int MatchingLinkCount,
    bool IsVisible,
    string? Href,
    string? Target,
    string? OpenedUrl,
    string? OpenedTitle);

public sealed record LeaveApplicationResult(
    DateTime StartDate,
    DateTime EndDate,
    DateTime HalfDayDate,
    string Description,
    string DescriptionValue,
    string HalfDayTextColor,
    string HalfDayBorderColor,
    string NotificationText);

public sealed record LeaveApplicationRequest(
    string LeaveType,
    string DescriptionPrefix,
    int StartMonthOffset,
    int RangeLengthDays,
    DayOfWeek StartDayOfWeek);

public sealed record LeaveCorrectionResult(
    DateTime StartDate,
    DateTime EndDate,
    string ExpectedDateRange,
    string CorrectionTypeValue,
    string DateRangeValue,
    string Description,
    string DescriptionValue,
    string HalfDayTextColor,
    string HalfDayBorderColor,
    string NotificationText);

public sealed record LeaveCorrectionRequest(
    string CorrectionType,
    string DescriptionPrefix);

public sealed record NavigationResult(
    string Destination,
    string? LinkHref,
    string? ExpectedUrlPath,
    string PreviousUrl,
    string CurrentUrl,
    string PageTitle,
    string DestinationMarkerText,
    bool DestinationIsVisible,
    bool DestinationIsActive,
    bool RequiresUrlChange);

public sealed record NavigationSectionResult(
    bool WasCollapsed,
    bool WasExpandedAfterReopen);
