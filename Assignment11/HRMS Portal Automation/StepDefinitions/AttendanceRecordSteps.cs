using System.Globalization;
using HRIntimeAutomation.Configuration;
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

    [When("I select a completed four-day period that includes Saturday and Sunday")]
    public async Task SelectCompletedFourDayPeriodContainingWeekend() =>
        _context.AttendanceDates = await _context.AttendanceRecordPage
            .SelectFourDayRangeContainingWeekendAsync();

    [When("I select a completed weekday period of {int} days")]
    public async Task SelectCompletedWeekdayPeriod(int rangeLengthDays) =>
        _context.AttendanceDates = await _context.AttendanceRecordPage
            .SelectRecentWeekdayRangeAsync(rangeLengthDays);

    [Then("one attendance record should appear for every selected date")]
    public async Task OneAttendanceRecordAppearsForEachSelectedDate()
    {
        await AssertAttendancePageAndSelectedRangeAsync();
        await Assertions.Expect(_context.AttendanceRecordPage.AttendanceRows)
            .ToHaveCountAsync(_context.AttendanceDates.Count);

        foreach (var date in _context.AttendanceDates)
        {
            var row = _context.AttendanceRecordPage.RowForDate(date);
            await Assertions.Expect(row).ToHaveCountAsync(1);
            await Assertions.Expect(row).ToBeVisibleAsync();
        }
    }

    [Then("the Weekly Off filter should show only the selected Saturday and Sunday")]
    public async Task WeeklyOffFilterShowsOnlySelectedWeekendDates()
    {
        var weekendDates = _context.AttendanceDates
            .Where(date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            .ToArray();
        Assert.That(weekendDates, Has.Length.EqualTo(AttendanceTestData.FilteredWeekendRecordCount));

        foreach (var weekendDate in weekendDates)
        {
            await Assertions.Expect(_context.AttendanceRecordPage.RowForDate(weekendDate))
                .ToContainTextAsync(AttendanceTestData.WeeklyOffRecordStatus);
        }

        await _context.AttendanceRecordPage.ApplyStatusFilterAsync(AttendanceTestData.WeeklyOffFilter);
        await Assertions.Expect(_context.AttendanceRecordPage.StatusField)
            .ToHaveValueAsync(AttendanceTestData.WeeklyOffFilter);
        await Assertions.Expect(_context.AttendanceRecordPage.AttendanceRows)
            .ToHaveCountAsync(weekendDates.Length);

        foreach (var weekendDate in weekendDates)
        {
            var row = _context.AttendanceRecordPage.RowForDate(weekendDate);
            await Assertions.Expect(row).ToHaveCountAsync(1);
            await Assertions.Expect(row).ToContainTextAsync(AttendanceTestData.WeeklyOffRecordStatus);
        }
    }

    [Then("no attendance record should fall outside the selected period")]
    public async Task NoAttendanceRecordFallsOutsideSelectedPeriod()
    {
        await AssertAttendancePageAndSelectedRangeAsync();
        var expectedLabels = _context.AttendanceDates
            .Select(date => date.ToString(AttendanceTestData.RecordDateFormat, CultureInfo.InvariantCulture))
            .ToHashSet(StringComparer.Ordinal);
        var rows = await _context.AttendanceRecordPage.AttendanceRows.AllInnerTextsAsync();

        Assert.Multiple(() =>
        {
            foreach (var row in rows)
            {
                Assert.That(expectedLabels.Any(row.Contains), Is.True,
                    $"Attendance row was outside the selected period: {row}");
            }
        });
    }

    private async Task AssertAttendancePageAndSelectedRangeAsync()
    {
        Assert.That(_context.AttendanceDates, Is.Not.Empty,
            "The attendance period was not saved by the When step.");
        await _context.AttendanceRecordPage.Pages.AssertUrlAsync(AppRoutes.AttendanceRecord);
        var expectedRange = $"{_context.AttendanceDates.First().ToString(AttendanceTestData.RecordDateFormat, CultureInfo.InvariantCulture)}" +
            $"{AttendanceTestData.RangeSeparator}" +
            $"{_context.AttendanceDates.Last().ToString(AttendanceTestData.RecordDateFormat, CultureInfo.InvariantCulture)}";
        await Assertions.Expect(_context.AttendanceRecordPage.DateRangeField).ToHaveValueAsync(expectedRange);
    }
}
