using System.Globalization;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class AttendanceRecordSteps
{
    private readonly ScenarioTestContext _context;

    public AttendanceRecordSteps(ScenarioTestContext context) => _context = context;

    [When("I select a four day attendance range containing Saturday and Sunday")]
    public async Task WhenISelectAFourDayAttendanceRangeContainingSaturdayAndSunday() =>
        _context.AttendanceRange = await _context.AttendanceRecordPage
            .SelectFourDayRangeContainingWeekendAsync();

    [Then("the attendance records and Weekly Off filter should be correct")]
    public async Task ThenTheAttendanceRecordsAndWeeklyOffFilterShouldBeCorrect()
    {
        Assert.That(_context.AttendanceRange, Is.Not.Null,
            "The attendance date range was not saved by the When step.");
        var selection = _context.AttendanceRange!;
        var expectedRange = $"{selection.StartDate.ToString(AttendanceTestData.RecordDateFormat, CultureInfo.InvariantCulture)}" +
            $"{AttendanceTestData.RangeSeparator}" +
            $"{selection.EndDate.ToString(AttendanceTestData.RecordDateFormat, CultureInfo.InvariantCulture)}";

        await Assertions.Expect(_context.AttendanceRecordPage.DateRangeField)
            .ToHaveValueAsync(expectedRange);
        await Assertions.Expect(_context.AttendanceRecordPage.AttendanceRows)
            .ToHaveCountAsync(AttendanceTestData.SelectedRangeRecordCount);

        foreach (var date in selection.ExpectedDates)
        {
            var row = _context.AttendanceRecordPage.RowForDate(date);
            await Assertions.Expect(row).ToHaveCountAsync(1);
            await Assertions.Expect(row).ToBeVisibleAsync();
        }

        foreach (var weekendDate in selection.ExpectedDates
                     .Where(date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday))
        {
            await Assertions.Expect(_context.AttendanceRecordPage.RowForDate(weekendDate))
                .ToContainTextAsync(AttendanceTestData.WeeklyOffRecordStatus);
        }

        await _context.AttendanceRecordPage.ApplyStatusFilterAsync(AttendanceTestData.WeeklyOffFilter);
        await Assertions.Expect(_context.AttendanceRecordPage.StatusField)
            .ToHaveValueAsync(AttendanceTestData.WeeklyOffFilter);
        await Assertions.Expect(_context.AttendanceRecordPage.AttendanceRows)
            .ToHaveCountAsync(AttendanceTestData.FilteredWeekendRecordCount);

        foreach (var weekendDate in selection.ExpectedDates
                     .Where(date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday))
        {
            var row = _context.AttendanceRecordPage.RowForDate(weekendDate);
            await Assertions.Expect(row).ToHaveCountAsync(1);
            await Assertions.Expect(row).ToContainTextAsync(AttendanceTestData.WeeklyOffRecordStatus);
        }
    }
}
