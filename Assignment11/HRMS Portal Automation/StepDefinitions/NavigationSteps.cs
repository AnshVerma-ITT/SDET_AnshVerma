using HRIntimeAutomation.Context;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class NavigationSteps
{
    private readonly ScenarioTestContext _context;

    public NavigationSteps(ScenarioTestContext context) => _context = context;

    [When("I verify all required left navigation links")]
    public async Task WhenIVerifyAllRequiredLeftNavigationLinks()
    {
        _context.NavigationResults.Clear();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenDashboardAsync());

        await _context.NavigationPage.OpenOrganizationAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenMyProfileAsync());
        await _context.NavigationPage.OpenOrganizationAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenEmployeeDirectoryAsync());

        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenAttendanceRecordAsync());
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenLeavesApplicationAsync());
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenLeaveEntitlementsAsync());
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenLeaveCorrectionAsync());
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenMyHolidaysAsync());
    }

    [Then("each required navigation page should open successfully")]
    public void ThenEachRequiredNavigationPageShouldOpenSuccessfully()
    {
        Assert.That(_context.NavigationResults, Is.Not.Empty,
            "No navigation results were captured by the When step.");

        Assert.Multiple(() =>
        {
            foreach (var result in _context.NavigationResults)
            {
                Assert.That(result.DestinationIsVisible, Is.True,
                    $"{result.Destination} did not display its destination marker.");

                if (result.RequiresUrlChange)
                {
                    Assert.That(result.CurrentUrl, Is.Not.EqualTo(result.PreviousUrl),
                        $"{result.Destination} did not change the URL.");
                }
            }
        });
    }

    [When("I navigate to Leaves Application")]
    public async Task WhenINavigateToLeavesApplication()
    {
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        await _context.NavigationPage.OpenLeavesApplicationAsync();
    }

    [When("I navigate to Attendance Record")]
    public async Task WhenINavigateToAttendanceRecord()
    {
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        await _context.NavigationPage.OpenAttendanceRecordAsync();
    }

    [When("I navigate to Leave Correction")]
    public async Task WhenINavigateToLeaveCorrection()
    {
        await _context.NavigationPage.OpenLeaveAndAttendanceAsync();
        await _context.NavigationPage.OpenLeaveCorrectionAsync();
    }
}
