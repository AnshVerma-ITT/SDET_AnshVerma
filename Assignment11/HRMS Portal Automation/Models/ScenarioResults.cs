namespace HRIntimeAutomation.Models;

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
    string? OpenedUrl);

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
    string PreviousUrl,
    string CurrentUrl,
    bool DestinationIsVisible,
    bool RequiresUrlChange);
