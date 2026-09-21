using System.Globalization;
using System.Text.RegularExpressions;
using HRIntimeAutomation.Models;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class LeaveCorrectionPage : BasePage
{
    private const string ApplyCorrectionButtonName = "Apply Correction";
    private const string CorrectionTypeLabel = "Correction Type";
    private const string DateRangeLabel = "Select Range";
    private const string DescriptionPlaceholder = "Type here";
    private const string SubmitButtonName = "Submit";
    private const string CalendarDropdownSelector = ".mantine-DateRangePicker-dropdown:visible";
    private const string CalendarDaySelector = "button.mantine-DateRangePicker-day:not([data-outside='true'])";
    private const string BadgeAncestorSelector = "xpath=ancestor::div[contains(@class,'mantine-Badge-root')][1]";
    private const string ChipDateFormat = "d-MMM-yyyy";
    private const string RecordDateFormat = "dd-MMM-yyyy";
    private const string DescriptionTimestampFormat = "yyyyMMdd_HHmmss";
    private const string CorrectionTypeOmission = "correction type";
    private const string DateOmission = "date";
    private const string TextColorScript = "element => getComputedStyle(element).color";
    private const string BorderColorScript = "element => getComputedStyle(element).borderColor";
    private const string ValidationSelector = ".mantine-InputWrapper-error:visible, [data-error='true']:visible";
    private const int ValidationResponseTimeoutMilliseconds = 3000;

    public LeaveCorrectionPage(IPage page) : base(page) { }

    private ILocator ApplyCorrectionButton => Page.GetByRole(AriaRole.Button,
        new() { Name = ApplyCorrectionButtonName, Exact = true });
    private ILocator CorrectionTypeField => Page.GetByLabel(CorrectionTypeLabel, new() { Exact = true });
    private ILocator DateRangeField => Page.GetByLabel(DateRangeLabel, new() { Exact = true });
    private ILocator DescriptionField => Page.GetByPlaceholder(DescriptionPlaceholder, new() { Exact = true });
    private ILocator SubmitButton => Page.GetByRole(AriaRole.Button,
        new() { Name = SubmitButtonName, Exact = true });
    private ILocator CalendarDropdown => Page.Locator(CalendarDropdownSelector);
    private ILocator Notification => Page.GetByRole(AriaRole.Alert).Last;
    public ILocator ApplyCorrectionDialog => Page.GetByRole(AriaRole.Dialog,
        new() { Name = ApplyCorrectionButtonName, Exact = true });

    public async Task<LeaveCorrectionResult> ApplyCorrectionAsync(
        LeaveCorrectionRequest request,
        int responseTimeoutMilliseconds)
    {
        await ApplyCorrectionButton.ClickAsync();
        await CorrectionTypeField.ClickAsync();
        await Page.GetByRole(AriaRole.Option,
            new() { Name = request.CorrectionType, Exact = true }).ClickAsync();

        await DateRangeField.ClickAsync();
        await CalendarDropdown.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var (startDate, endDate) = await FindLatestTwoDayPastRangeAsync();
        await CurrentMonthDay(startDate.Day).ClickAsync();
        await CurrentMonthDay(endDate.Day).ClickAsync();

        var chipText = startDate.ToString(ChipDateFormat, CultureInfo.InvariantCulture);
        var chipDate = Page.GetByText(chipText, new() { Exact = true });
        await chipDate.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var chipBadge = chipDate.Locator(BadgeAncestorSelector);
        await chipBadge.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await chipDate.ClickAsync();

        var description = $"{request.DescriptionPrefix}_{DateTime.Now.ToString(DescriptionTimestampFormat, CultureInfo.InvariantCulture)}";
        await DescriptionField.FillAsync(description);

        var correctionTypeValue = await CorrectionTypeField.InputValueAsync();
        var dateRangeValue = await DateRangeField.InputValueAsync();
        var descriptionValue = await DescriptionField.InputValueAsync();
        var halfDayTextColor = await chipBadge.EvaluateAsync<string>(TextColorScript);
        var halfDayBorderColor = await chipBadge.EvaluateAsync<string>(BorderColorScript);

        await SubmitButton.ClickAsync();
        await WaitForSubmissionNotificationAsync(responseTimeoutMilliseconds);

        return new LeaveCorrectionResult(
            startDate,
            endDate,
            $"{startDate.ToString(RecordDateFormat, CultureInfo.InvariantCulture)} to {endDate.ToString(RecordDateFormat, CultureInfo.InvariantCulture)}",
            correctionTypeValue,
            dateRangeValue,
            description,
            descriptionValue,
            halfDayTextColor,
            halfDayBorderColor,
            (await Notification.InnerTextAsync()).Trim());
    }

    public ILocator CreatedRecord(string expectedDateRange, string correctionType) =>
        Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = expectedDateRange })
            .Filter(new() { HasText = correctionType })
            .First;

    public async Task<FormValidationResult> SubmitIncompleteCorrectionAsync(
        string omittedField,
        LeaveCorrectionRequest request)
    {
        await ApplyCorrectionButton.ClickAsync();

        if (!omittedField.Equals(CorrectionTypeOmission, StringComparison.OrdinalIgnoreCase))
        {
            await CorrectionTypeField.ClickAsync();
            await Page.GetByRole(AriaRole.Option,
                new() { Name = request.CorrectionType, Exact = true }).ClickAsync();
        }

        if (!omittedField.Equals(DateOmission, StringComparison.OrdinalIgnoreCase))
        {
            await DateRangeField.ClickAsync();
            await CalendarDropdown.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var (startDate, endDate) = await FindLatestTwoDayPastRangeAsync();
            await CurrentMonthDay(startDate.Day).ClickAsync();
            await CurrentMonthDay(endDate.Day).ClickAsync();
        }

        await DescriptionField.FillAsync($"{request.DescriptionPrefix}_VALIDATION");

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
            ApplyCorrectionButtonName,
            omittedField,
            submitButtonEnabled,
            await ApplyCorrectionDialog.IsVisibleAsync(),
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
            if (!await ApplyCorrectionDialog.IsVisibleAsync() || !await SubmitButton.IsEnabledAsync())
                throw;

            await SubmitButton.ClickAsync();
        }

        await Notification.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = Math.Max(1000, responseTimeoutMilliseconds - firstWait)
        });
    }

    private async Task<(DateTime Start, DateTime End)> FindLatestTwoDayPastRangeAsync()
    {
        var today = DateTime.Today;
        var currentMonthStart = new DateTime(today.Year, today.Month, 1);

        for (var endDay = today.Day - 1; endDay >= 2; endDay--)
        {
            var startDay = endDay - 1;
            var start = currentMonthStart.AddDays(startDay - 1);
            var end = currentMonthStart.AddDays(endDay - 1);

            if (await IsEnabledCurrentMonthDayAsync(startDay) && await IsEnabledCurrentMonthDayAsync(endDay))
                return (start, end);
        }

        throw new InvalidOperationException(
            "Could not find two consecutive enabled past dates in the current month for Leave Correction.");
    }

    private async Task<bool> IsEnabledCurrentMonthDayAsync(int day)
    {
        var locator = CurrentMonthDay(day);
        return await locator.CountAsync() == 1 && await locator.IsEnabledAsync();
    }

    private ILocator CurrentMonthDay(int day)
    {
        var dayText = Regex.Escape(day.ToString(CultureInfo.InvariantCulture));
        return CalendarDropdown.Locator(CalendarDaySelector)
            .Filter(new() { HasTextRegex = new Regex($"^{dayText}$") });
    }
}
