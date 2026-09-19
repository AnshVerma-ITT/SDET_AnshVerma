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
    }
}
