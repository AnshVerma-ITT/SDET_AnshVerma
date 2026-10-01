namespace HRIntimeAutomation.Models;

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
