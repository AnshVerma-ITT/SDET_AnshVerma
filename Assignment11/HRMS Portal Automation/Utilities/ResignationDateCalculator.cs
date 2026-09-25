namespace HRIntimeAutomation.Utilities;

public static class ResignationDateCalculator
{
    public static DateTime CalculateLastWorkingDate(
        DateTime dateOfApply,
        int noticePeriodMonths,
        int lastWorkingDayOffset) =>
        dateOfApply.AddMonths(noticePeriodMonths).AddDays(lastWorkingDayOffset);
}
