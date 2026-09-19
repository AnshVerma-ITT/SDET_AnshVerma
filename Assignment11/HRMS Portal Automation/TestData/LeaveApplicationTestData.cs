using HRIntimeAutomation.Models;

namespace HRIntimeAutomation.TestData;

public static class LeaveApplicationTestData
{
    public const string LeaveType = "Casual Leave";
    public const string DescriptionPrefix = "HRMS_LEAVE_AUTOMATION";
    public const string OperationSuccessMessage = "Operation completed successfully.";
    public const string LeaveSuccessMessage = "Leave apply successfully.";
    public const string HalfDayColor = "rgb(252, 88, 71)";
    public const int StartMonthOffset = 1;
    public const int RangeLengthDays = 7;
    public const DayOfWeek StartDayOfWeek = DayOfWeek.Friday;

    public static readonly LeaveApplicationRequest Request = new(
        LeaveType,
        DescriptionPrefix,
        StartMonthOffset,
        RangeLengthDays,
        StartDayOfWeek);
}
