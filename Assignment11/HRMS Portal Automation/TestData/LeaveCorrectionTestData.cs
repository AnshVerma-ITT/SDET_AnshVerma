using HRIntimeAutomation.Models;

namespace HRIntimeAutomation.TestData;

public static class LeaveCorrectionTestData
{
    public const string CorrectionType = "Work from home";
    public const string DescriptionPrefix = "HRMS_CORRECTION_AUTOMATION";
    public const string SuccessMessage = "Leave correction apply successfully.";
    public const string ExpectedDuration = "1.5";
    public const string HalfDayColor = "rgb(252, 88, 71)";
    public const string CorrectionTypeField = "correction type";
    public const string DateField = "date";
    public const int StatusCellIndex = 3;
    public const int MinimumExpectedCellCount = StatusCellIndex + 1;

    public static readonly LeaveCorrectionRequest Request = new(CorrectionType, DescriptionPrefix);
}
