namespace HRIntimeAutomation.Models;

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

public sealed record LeaveCorrectionRequest(string CorrectionType, string DescriptionPrefix);
