namespace HRIntimeAutomation.TestData;

public static class ResignationTestData
{
    public const string DateFormat = "dd-MMM-yyyy";
    public const string BoundaryInputFormat = "yyyy-MM-dd";
    public const string DateValuePattern = @"^\d{2}-[A-Za-z]{3}-\d{4}$";
    public const int NoticePeriodMonths = 2;
    public const int LastWorkingDayOffset = -1;
}
