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
                Assert.That(result.ExpectedUrlPath, Is.Not.Null.And.Not.Empty,
                    $"{result.Destination} expected URL could not be calculated.");
                Assert.That(result.PageTitle, Is.Not.Empty,
                    $"{result.Destination} did not expose a page title.");
                Assert.That(result.DestinationMarkerText, Does.Contain(result.Destination).IgnoreCase,
                    $"{result.Destination} destination marker text was incorrect.");
                Assert.That(result.DestinationIsActive, Is.True,
                    $"{result.Destination} was not identifiable as the current navigation item.");

                if (Uri.TryCreate(result.CurrentUrl, UriKind.Absolute, out var currentUri))
                {
                    Assert.That(currentUri.AbsolutePath.TrimEnd('/'),
                        Is.EqualTo(result.ExpectedUrlPath!.TrimEnd('/')).IgnoreCase,
                        $"{result.Destination} opened an unexpected URL.");
                }
                else
                {
                    Assert.Fail($"{result.Destination} produced an invalid URL: {result.CurrentUrl}");
                }

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

    [When("I refresh the current page and collapse and reopen Organization navigation")]
    public async Task WhenIRefreshAndToggleOrganizationNavigation()
    {
        await _context.NavigationPage.ReloadAsync();
        _context.NavigationSectionResult = await _context.NavigationPage
            .CollapseAndReopenOrganizationAsync();
        _context.NavigationResults.Clear();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenMyProfileAsync());
    }

    [Then("navigation should remain usable with correct expanded and collapsed states")]
    public void ThenNavigationShouldRemainUsableAfterRefresh()
    {
        Assert.That(_context.NavigationSectionResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(_context.NavigationSectionResult!.WasCollapsed, Is.True,
                "Organization navigation did not collapse.");
            Assert.That(_context.NavigationSectionResult.WasExpandedAfterReopen, Is.True,
                "Organization navigation did not reopen.");
        });

        ThenEachRequiredNavigationPageShouldOpenSuccessfully();
    }
}
