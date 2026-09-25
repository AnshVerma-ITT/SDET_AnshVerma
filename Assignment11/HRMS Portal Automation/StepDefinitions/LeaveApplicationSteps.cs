using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class LeaveApplicationSteps
{
    private readonly ScenarioTestContext _context;

    public LeaveApplicationSteps(ScenarioTestContext context) => _context = context;

    [When("I apply for Casual Leave using the configured date range")]
    public async Task WhenIApplyForCasualLeaveUsingTheConfiguredDateRange() =>
        _context.LeaveApplicationResult = await _context.LeaveApplicationPage.ApplyLeaveAsync(
            LeaveApplicationTestData.Request,
            _context.Driver.Settings.ResponseTimeoutMilliseconds);

    [When("I submit a Leave Application without {string}")]
    public async Task WhenISubmitALeaveApplicationWithout(string omittedField) =>
        _context.FormValidationResult = await _context.LeaveApplicationPage
            .SubmitIncompleteLeaveAsync(omittedField, LeaveApplicationTestData.Request);

    [Then("the leave request success message should be displayed")]
    public async Task ThenTheLeaveRequestSuccessMessageShouldBeDisplayed()
    {
        Assert.That(_context.LeaveApplicationResult, Is.Not.Null,
            "The leave application result was not saved by the When step.");
        var result = _context.LeaveApplicationResult!;

        Assert.Multiple(() =>
        {
            Assert.That(result.DescriptionValue, Is.EqualTo(result.Description));
            Assert.That(result.HalfDayTextColor, Is.EqualTo(LeaveApplicationTestData.HalfDayColor));
            Assert.That(result.HalfDayBorderColor, Is.EqualTo(LeaveApplicationTestData.HalfDayColor));
            Assert.That(result.NotificationText,
                Does.Contain(LeaveApplicationTestData.OperationSuccessMessage).IgnoreCase);
            Assert.That(result.NotificationText,
                Does.Contain(LeaveApplicationTestData.LeaveSuccessMessage).IgnoreCase);
        });

        await Assertions.Expect(_context.LeaveApplicationPage.ApplyLeaveDialog).ToBeHiddenAsync();

        var createdRecord = _context.LeaveApplicationPage.CreatedRecord(
            result.StartDate,
            result.EndDate,
            LeaveApplicationTestData.LeaveType);
        await Assertions.Expect(createdRecord).ToBeVisibleAsync();
        await Assertions.Expect(createdRecord).ToContainTextAsync(LeaveApplicationTestData.LeaveType);
        await Assertions.Expect(createdRecord).ToContainTextAsync(
            result.StartDate.ToString(AttendanceTestData.RecordDateFormat));
        await Assertions.Expect(createdRecord).ToContainTextAsync(
            result.EndDate.ToString(AttendanceTestData.RecordDateFormat));

        var rowText = await createdRecord.InnerTextAsync();
        Assert.That(LeaveApplicationTestData.AllowedStatuses.Any(status =>
                rowText.Contains(status, StringComparison.OrdinalIgnoreCase)),
            Is.True,
            $"The created Leave Application record has no recognized status. Row: {rowText}");
    }

    [Then("the incomplete Leave Application should be rejected")]
    public void ThenTheIncompleteLeaveApplicationShouldBeRejected()
    {
        Assert.That(_context.FormValidationResult, Is.Not.Null);
        var result = _context.FormValidationResult!;
        var hasValidationEvidence = !result.SubmitButtonEnabled
            || result.ValidationMessages.Count > 0
            || !string.IsNullOrWhiteSpace(result.NotificationText);

        Assert.Multiple(() =>
        {
            Assert.That(result.DialogVisible, Is.True,
                "The incomplete Leave Application dialog closed as if submission succeeded.");
            Assert.That(result.NotificationText ?? string.Empty,
                Does.Not.Contain(LeaveApplicationTestData.LeaveSuccessMessage).IgnoreCase);
            Assert.That(hasValidationEvidence, Is.True,
                $"No validation was exposed when '{result.OmittedField}' was omitted.");
        });
    }
}
