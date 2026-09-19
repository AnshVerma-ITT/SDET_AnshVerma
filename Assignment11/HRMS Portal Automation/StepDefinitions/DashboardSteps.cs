using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class DashboardSteps
{
    private readonly ScenarioTestContext _context;

    public DashboardSteps(ScenarioTestContext context) => _context = context;

    [When("I navigate to the Dashboard")]
    public Task WhenINavigateToTheDashboard() => _context.DashboardPage.NavigateToDashboardAsync();

    [Then("the calendar should show the current date month and year")]
    public async Task ThenTheCalendarShouldShowTheCurrentDateMonthAndYear()
    {
        await _context.DashboardPage.HoverCalendarAsync();
        await Assertions.Expect(_context.DashboardPage.CalendarHeader)
            .ToContainTextAsync(_context.DashboardPage.ExpectedMonthYear);
        await Assertions.Expect(_context.DashboardPage.CurrentDay).ToBeVisibleAsync();
        await Assertions.Expect(_context.DashboardPage.CurrentDay)
            .ToHaveCSSAsync("background-color", DashboardTestData.TodayBackgroundColor);
    }
}
