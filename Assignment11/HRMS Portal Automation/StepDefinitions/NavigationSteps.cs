using HRIntimeAutomation.Context;
using HRIntimeAutomation.Models;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class NavigationSteps
{
    private readonly ScenarioTestContext _context;
    public NavigationSteps(ScenarioTestContext context) => _context = context;

    [When("I verify all required left navigation links")]
    public async Task VerifyAllLinks()
    {
        _context.NavigationResults.Clear();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenDashboardAsync());
        foreach (var pageName in new[] { "Organization My Profile", "Employee Directory", "Attendance Record", "Leaves Application", "Leave Entitlements", "Leave Correction", "My Holidays" })
            _context.NavigationResults.Add(await NavigateAsync(pageName));
    }

    [When("I navigate to {string}")]
    public Task NavigateTo(string pageName) => NavigateAsync(pageName);

    [Then("each required navigation page should open successfully")]
    public void AllPagesOpened()
    {
        Assert.That(_context.NavigationResults, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var result in _context.NavigationResults)
            {
                Assert.That(result.DestinationIsVisible, Is.True, $"{result.Destination} marker was not visible.");
                Assert.That(result.DestinationIsActive, Is.True, $"{result.Destination} was not active.");
                Assert.That(result.PageTitle, Is.Not.Empty);
                Assert.That(_context.NavigationPage.Pages.RouteMatches(result.CurrentUrl, result.ExpectedRoute), Is.True,
                    $"{result.Destination} opened '{result.CurrentUrl}', expected '{result.ExpectedRoute}'.");
            }
        });
    }

    [When("I refresh the current page and collapse and reopen Organization navigation")]
    public async Task RefreshAndToggleOrganization()
    {
        await _context.NavigationPage.ReloadAsync();
        var state = await _context.NavigationPage.CollapseAndReopenOrganizationAsync();
        _context.OrganizationWasCollapsed = state.WasCollapsed;
        _context.OrganizationWasReopened = state.WasReopened;
        _context.NavigationResults.Clear();
        _context.NavigationResults.Add(await _context.NavigationPage.OpenMyProfileAsync());
    }

    [Then("navigation should remain usable with correct expanded and collapsed states")]
    public void NavigationRemainsUsable()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_context.OrganizationWasCollapsed, Is.True);
            Assert.That(_context.OrganizationWasReopened, Is.True);
        });
        AllPagesOpened();
    }

    private async Task<NavigationResult> NavigateAsync(string pageName)
    {
        switch (pageName)
        {
            case "Dashboard": return await _context.NavigationPage.OpenDashboardAsync();
            case "Organization My Profile": await _context.NavigationPage.OpenOrganizationAsync(); return await _context.NavigationPage.OpenMyProfileAsync();
            case "Employee Directory": await _context.NavigationPage.OpenOrganizationAsync(); return await _context.NavigationPage.OpenEmployeeDirectoryAsync();
            case "Attendance Record": await _context.NavigationPage.OpenLeaveAndAttendanceAsync(); return await _context.NavigationPage.OpenAttendanceRecordAsync();
            case "Leaves Application": await _context.NavigationPage.OpenLeaveAndAttendanceAsync(); return await _context.NavigationPage.OpenLeavesApplicationAsync();
            case "Leave Entitlements": await _context.NavigationPage.OpenLeaveAndAttendanceAsync(); return await _context.NavigationPage.OpenLeaveEntitlementsAsync();
            case "Leave Correction": await _context.NavigationPage.OpenLeaveAndAttendanceAsync(); return await _context.NavigationPage.OpenLeaveCorrectionAsync();
            case "My Holidays": await _context.NavigationPage.OpenLeaveAndAttendanceAsync(); return await _context.NavigationPage.OpenMyHolidaysAsync();
            default: throw new ArgumentOutOfRangeException(nameof(pageName), pageName, "Unsupported HRMS page name.");
        }
    }
}
