using System.Globalization;
using System.Text.RegularExpressions;
using HRIntimeAutomation.Models;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class LeaveApplicationPage : BasePage
{
    private const string ApplyLeaveButtonName = "Apply Leave";
    private const string LeaveTypeLabel = "Leave Type";
    private const string DateRangeLabel = "Select Range";
    private const string DescriptionPlaceholder = "Type here";
    private const string SubmitButtonName = "Submit";
    private const string CalendarDropdownSelector = ".mantine-DateRangePicker-dropdown";
    private const string CalendarHeaderSelector = "button.mantine-DateRangePicker-calendarHeaderLevel";
    private const string CalendarControlSelector = "button.mantine-DateRangePicker-calendarHeaderControl";
    private const string CalendarDaySelector = "button.mantine-DateRangePicker-day:not([data-outside='true'])";
    private const string BadgeAncestorSelector = "xpath=ancestor::div[contains(@class,'mantine-Badge-root')][1]";
    private const string MonthYearFormat = "MMMM yyyy";
    private const string ChipDateFormat = "d MMM yyyy";
    private const string DescriptionTimestampFormat = "yyyyMMdd_HHmmss";
    private const string RecordDateFormat = "dd-MMM-yyyy";
    private const string LeaveTypeOmission = "leave type";
    private const string DateOmission = "date";
    private const string TextColorScript = "element => getComputedStyle(element).color";
    private const string BorderColorScript = "element => getComputedStyle(element).borderColor";
    private const string ValidationSelector = ".mantine-InputWrapper-error:visible, [data-error='true']:visible";
    private const int ValidationResponseTimeoutMilliseconds = 3000;
    private const int CalendarNavigationAttemptLimit = 14;

    public LeaveApplicationPage(IPage page) : base(page) { }

    private ILocator ApplyLeaveButton => Page.GetByRole(AriaRole.Button,
        new() { Name = ApplyLeaveButtonName, Exact = true });
    private ILocator LeaveTypeField => Page.GetByLabel(LeaveTypeLabel, new() { Exact = true });
    private ILocator DateRangeField => Page.GetByLabel(DateRangeLabel, new() { Exact = true });
    private ILocator DescriptionField => Page.GetByPlaceholder(DescriptionPlaceholder, new() { Exact = true });
    private ILocator SubmitButton => Page.GetByRole(AriaRole.Button,
        new() { Name = SubmitButtonName, Exact = true });
    private ILocator CalendarDropdown => Page.Locator(CalendarDropdownSelector).Filter(new() { Visible = true });
    private ILocator Notification => Page.GetByRole(AriaRole.Alert).Last;
    public ILocator ApplyLeaveDialog => Page.GetByRole(AriaRole.Dialog,
        new() { Name = ApplyLeaveButtonName, Exact = true });

    public async Task<LeaveApplicationResult> ApplyLeaveAsync(
        LeaveApplicationRequest request,
        int responseTimeoutMilliseconds)
    {
        await ApplyLeaveButton.ClickAsync();
        await LeaveTypeField.ClickAsync();
        await Page.GetByRole(AriaRole.Option,
            new() { Name = request.LeaveType, Exact = true }).ClickAsync();

        var firstOfTargetMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            .AddMonths(request.StartMonthOffset);
        var daysUntilStartDay = ((int)request.StartDayOfWeek - (int)firstOfTargetMonth.DayOfWeek + 7) % 7;
        var startDate = firstOfTargetMonth.AddDays(daysUntilStartDay);
        var endDate = startDate.AddDays(request.RangeLengthDays - 1);

        await DateRangeField.ClickAsync();
        await SelectCalendarDateAsync(startDate);
        await SelectCalendarDateAsync(endDate);

        var lastApplicableDate = Enumerable.Range(0, request.RangeLengthDays)
            .Select(offset => startDate.AddDays(offset))
            .Where(date => date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            .Last();
        var halfDayBadge = await MarkDateAsHalfDayAsync(lastApplicableDate);

        var description = $"{request.DescriptionPrefix}_{DateTime.Now.ToString(DescriptionTimestampFormat, CultureInfo.InvariantCulture)}";
        await DescriptionField.FillAsync(description);
        var descriptionValue = await DescriptionField.InputValueAsync();
        var halfDayTextColor = await halfDayBadge.EvaluateAsync<string>(TextColorScript);
        var halfDayBorderColor = await halfDayBadge.EvaluateAsync<string>(BorderColorScript);

        await SubmitButton.ClickAsync();
        await WaitForSubmissionNotificationAsync(responseTimeoutMilliseconds);

        return new LeaveApplicationResult(
            startDate,
            endDate,
            lastApplicableDate,
            description,
            descriptionValue,
            halfDayTextColor,
            halfDayBorderColor,
            (await Notification.InnerTextAsync()).Trim());
    }

    public ILocator CreatedRecord(DateTime startDate, DateTime endDate, string leaveType) =>
        Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = startDate.ToString(RecordDateFormat, CultureInfo.InvariantCulture) })
            .Filter(new() { HasText = endDate.ToString(RecordDateFormat, CultureInfo.InvariantCulture) })
            .Filter(new() { HasText = leaveType })
            .First;

    public async Task<FormValidationResult> SubmitIncompleteLeaveAsync(
        string omittedField,
        LeaveApplicationRequest request)
    {
        await ApplyLeaveButton.ClickAsync();

        if (!omittedField.Equals(LeaveTypeOmission, StringComparison.OrdinalIgnoreCase))
        {
            await LeaveTypeField.ClickAsync();
            await Page.GetByRole(AriaRole.Option,
                new() { Name = request.LeaveType, Exact = true }).ClickAsync();
        }

        if (!omittedField.Equals(DateOmission, StringComparison.OrdinalIgnoreCase))
        {
            var firstOfTargetMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
                .AddMonths(request.StartMonthOffset);
            var daysUntilStartDay = ((int)request.StartDayOfWeek - (int)firstOfTargetMonth.DayOfWeek + 7) % 7;
            var selectedDate = firstOfTargetMonth.AddDays(daysUntilStartDay);
            await DateRangeField.ClickAsync();
            await SelectCalendarDateAsync(selectedDate);
            await SelectCalendarDateAsync(selectedDate);
        }

        var description = $"{request.DescriptionPrefix}_VALIDATION";
        await DescriptionField.FillAsync(description);

        var submitButtonEnabled = await SubmitButton.IsEnabledAsync();
        string? notificationText = null;
        if (submitButtonEnabled)
        {
            await SubmitButton.ClickAsync();
            try
            {
                await Notification.WaitForAsync(new()
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = ValidationResponseTimeoutMilliseconds
                });
                notificationText = (await Notification.InnerTextAsync()).Trim();
            }
            catch (TimeoutException)
            {
                // Field-level validation may not create a notification.
            }
        }

        return new FormValidationResult(
            ApplyLeaveButtonName,
            omittedField,
            submitButtonEnabled,
            await ApplyLeaveDialog.IsVisibleAsync(),
            (await Page.Locator(ValidationSelector).AllInnerTextsAsync())
                .Select(message => message.Trim())
                .Where(message => message.Length > 0)
                .ToArray(),
            notificationText);
    }

    private async Task WaitForSubmissionNotificationAsync(int responseTimeoutMilliseconds)
    {
        // HRMS occasionally accepts the first UI click without starting the submit action.
        // Retry once only when the dialog is still open, no alert appeared, and Submit remains enabled.
        var firstWait = Math.Min(10000, responseTimeoutMilliseconds);
        try
        {
            await Notification.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
                Timeout = firstWait
            });
            return;
        }
        catch (TimeoutException)
        {
            if (!await ApplyLeaveDialog.IsVisibleAsync() || !await SubmitButton.IsEnabledAsync())
                throw;

            await SubmitButton.ClickAsync();
        }

        await Notification.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = Math.Max(1000, responseTimeoutMilliseconds - firstWait)
        });
    }

    private async Task<ILocator> MarkDateAsHalfDayAsync(DateTime targetDate)
    {
        var chipText = targetDate.ToString(ChipDateFormat, CultureInfo.InvariantCulture);
        var chipDate = Page.GetByText(chipText, new() { Exact = true });
        await chipDate.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var chipBadge = chipDate.Locator(BadgeAncestorSelector);
        await chipBadge.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await chipDate.ClickAsync();
        return chipBadge;
    }

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
}
