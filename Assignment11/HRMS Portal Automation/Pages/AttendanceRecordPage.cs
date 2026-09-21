using System.Globalization;
using System.Text.RegularExpressions;
using HRIntimeAutomation.Models;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class AttendanceRecordPage : BasePage
{
    private const string DateRangePlaceholder = "Select Date";
    private const string StatusPlaceholder = "Status";
    private const string CalendarDropdownSelector = ".mantine-DateRangePicker-dropdown";
    private const string CalendarHeaderSelector = "button.mantine-DateRangePicker-calendarHeaderLevel";
    private const string CalendarControlSelector = "button.mantine-DateRangePicker-calendarHeaderControl";
    private const string CalendarDaySelector = "button.mantine-DateRangePicker-day:not([data-outside='true'])";
    private const string AttendanceTableText = "Attendance";
    private const string AttendanceRowSelector = "tbody tr";
    private const string MonthYearFormat = "MMMM yyyy";
    private const string RecordDateFormat = "dd-MMM-yyyy";
    private const int CalendarNavigationAttemptLimit = 24;

    public AttendanceRecordPage(IPage page) : base(page) { }

    public ILocator DateRangeField => Page.GetByPlaceholder(DateRangePlaceholder, new() { Exact = true });
    public ILocator StatusField => Page.GetByPlaceholder(StatusPlaceholder, new() { Exact = true });
    public ILocator AttendanceRows => Page.GetByRole(AriaRole.Table)
        .Filter(new() { HasText = AttendanceTableText })
        .Locator(AttendanceRowSelector);

    private ILocator CalendarDropdown => Page.Locator(CalendarDropdownSelector).Filter(new() { Visible = true });

    public async Task<AttendanceRangeSelection> SelectFourDayRangeContainingWeekendAsync()
    {
        var today = DateTime.Today;
        var daysSinceSaturday = ((int)today.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
        var saturday = today.AddDays(-daysSinceSaturday);
        if (saturday.AddDays(3) >= today)
            saturday = saturday.AddDays(-7);

        var expectedDates = Enumerable.Range(0, 4)
            .Select(offset => saturday.AddDays(offset))
            .ToArray();

        await DateRangeField.ClickAsync();
        await SelectCalendarDateAsync(expectedDates.First());
        await SelectCalendarDateAsync(expectedDates.Last());

        return new AttendanceRangeSelection(expectedDates.First(), expectedDates.Last(), expectedDates);
    }

    public async Task<AttendanceRangeSelection> SelectRecentWeekdayRangeAsync(int rangeLengthDays)
    {
        if (rangeLengthDays <= 0 || rangeLengthDays > 5)
            throw new ArgumentOutOfRangeException(nameof(rangeLengthDays));

        var today = DateTime.Today;
        for (var endOffset = 1; endOffset <= 21; endOffset++)
        {
            var endDate = today.AddDays(-endOffset);
            var startDate = endDate.AddDays(-(rangeLengthDays - 1));
            var expectedDates = Enumerable.Range(0, rangeLengthDays)
                .Select(offset => startDate.AddDays(offset))
                .ToArray();

            if (expectedDates.All(date =>
                    date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday))
            {
                await SelectRangeAsync(expectedDates);
                return new AttendanceRangeSelection(startDate, endDate, expectedDates);
            }
        }

        throw new InvalidOperationException("Could not find a recent completed weekday attendance range.");
    }

    public async Task ApplyStatusFilterAsync(string status)
    {
        await StatusField.ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = status, Exact = true }).ClickAsync();
    }

    public ILocator RowForDate(DateTime date) => AttendanceRows.Filter(new()
    {
        HasText = date.ToString(RecordDateFormat, CultureInfo.InvariantCulture)
    });

    private async Task SelectCalendarDateAsync(DateTime targetDate)
    {
        await CalendarDropdown.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        for (var attempt = 0; attempt < CalendarNavigationAttemptLimit; attempt++)
        {
            var monthHeader = CalendarDropdown.Locator(CalendarHeaderSelector).First;
            var headerText = (await monthHeader.InnerTextAsync()).Trim();

            if (!DateTime.TryParseExact(headerText, MonthYearFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var displayedMonth))
            {
                throw new InvalidOperationException($"Could not read calendar month header: '{headerText}'.");
            }

            var targetMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
            var currentMonth = new DateTime(displayedMonth.Year, displayedMonth.Month, 1);
            if (currentMonth == targetMonth)
                break;

            var controls = CalendarDropdown.Locator(CalendarControlSelector);
            await (currentMonth < targetMonth ? controls.Last : controls.First).ClickAsync();
        }

        var dayText = Regex.Escape(targetDate.Day.ToString(CultureInfo.InvariantCulture));
        var day = CalendarDropdown.Locator(CalendarDaySelector)
            .Filter(new() { HasTextRegex = new Regex($"^{dayText}$") });

        await day.ClickAsync();
    }

    private async Task SelectRangeAsync(IReadOnlyList<DateTime> expectedDates)
    {
        await DateRangeField.ClickAsync();
        await SelectCalendarDateAsync(expectedDates.First());
        await SelectCalendarDateAsync(expectedDates.Last());
    }
}
