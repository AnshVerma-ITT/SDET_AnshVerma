using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Models;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class NavigationPage : BasePage
{
    private const string SiblingMenuSelector = "xpath=following-sibling::div[1]";
    private const string ExpandedValue = "false";
    private const string ActiveValue = "true";
    private const int MenuExpansionAttemptLimit = 20;

    public NavigationPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    private ILocator Dashboard => Page.Locator("div.mantine-bfmej7").Filter(new() { HasText = "Dashboard" });
    private ILocator OrganizationButton => Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Organization" });
    private ILocator OrganizationMenu => OrganizationButton.Locator(SiblingMenuSelector);
    private ILocator LeaveButton => Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Leave & Attendance" });
    private ILocator LeaveMenu => LeaveButton.Locator(SiblingMenuSelector);
    private ILocator OrganizationLink(string text) => OrganizationMenu.Locator("a").Filter(new() { HasText = text });
    private ILocator LeaveLink(string text) => LeaveMenu.Locator("a").Filter(new() { HasText = text });
    private ILocator Breadcrumb(string text) => Page.Locator("a.mantine-Text-root.mantine-Anchor-root, div.mantine-Breadcrumbs-breadcrumb")
        .Filter(new() { HasText = text });

    public async Task OpenOrganizationAsync()
    {
        if (!await IsExpandedAsync(OrganizationMenu))
            await OrganizationButton.ClickAsync();
        await WaitForExpandedAsync(OrganizationMenu);
    }

    public async Task OpenLeaveAndAttendanceAsync()
    {
        if (!await IsExpandedAsync(LeaveMenu))
            await LeaveButton.ClickAsync();
        await WaitForExpandedAsync(LeaveMenu);
    }

    public async Task<(bool WasCollapsed, bool WasReopened)> CollapseAndReopenOrganizationAsync()
    {
        await OpenOrganizationAsync();
        await OrganizationButton.ClickAsync();
        await OrganizationMenu.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        var collapsed = !await IsExpandedAsync(OrganizationMenu);
        await OpenOrganizationAsync();
        return (collapsed, await IsExpandedAsync(OrganizationMenu));
    }

    public Task ReloadAsync() => Pages.ReloadAsync();
    public Task<NavigationResult> OpenDashboardAsync() => OpenDestinationAsync("Dashboard", Dashboard, AppRoutes.Dashboard);
    public Task<NavigationResult> OpenMyProfileAsync() => OpenDestinationAsync("My profile", OrganizationLink("My profile"), AppRoutes.MyProfile);
    public Task<NavigationResult> OpenEmployeeDirectoryAsync() => OpenDestinationAsync("Employee directory", OrganizationLink("Employee directory"), AppRoutes.EmployeeDirectory);
    public Task<NavigationResult> OpenAttendanceRecordAsync() => OpenDestinationAsync("Attendance record", LeaveLink("Attendance record"), AppRoutes.AttendanceRecord);
    public Task<NavigationResult> OpenLeavesApplicationAsync() => OpenDestinationAsync("Leaves application", LeaveLink("Leaves application"), AppRoutes.LeavesApplication);
    public Task<NavigationResult> OpenLeaveEntitlementsAsync() => OpenDestinationAsync("Leave entitlements", LeaveLink("Leave entitlements"), AppRoutes.LeaveEntitlements);
    public Task<NavigationResult> OpenLeaveCorrectionAsync() => OpenDestinationAsync("Leave correction", LeaveLink("Leave correction"), AppRoutes.LeaveCorrection);
    public Task<NavigationResult> OpenMyHolidaysAsync() => OpenDestinationAsync("My holidays", LeaveLink("My holidays"), AppRoutes.MyHolidays);

    private async Task<NavigationResult> OpenDestinationAsync(string destination, ILocator link, string route)
    {
        await Pages.ClickAndAssertUrlAsync(link, route);
        var marker = Breadcrumb(destination);
        await marker.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var active = string.Equals(await link.GetAttributeAsync("aria-current"), "page", StringComparison.OrdinalIgnoreCase)
            || string.Equals(await link.GetAttributeAsync("data-active"), ActiveValue, StringComparison.OrdinalIgnoreCase)
            || ((await link.GetAttributeAsync("class"))?.Contains("active", StringComparison.OrdinalIgnoreCase) ?? false)
            || Pages.RouteMatches(Page.Url, route);
        return new(destination, route, Page.Url, await Page.TitleAsync(), (await marker.InnerTextAsync()).Trim(), await marker.IsVisibleAsync(), active);
    }

    private static async Task<bool> IsExpandedAsync(ILocator menu) =>
        string.Equals(await menu.GetAttributeAsync("aria-hidden"), ExpandedValue, StringComparison.OrdinalIgnoreCase)
        && await menu.IsVisibleAsync();

    private static async Task WaitForExpandedAsync(ILocator menu)
    {
        await menu.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        for (var attempt = 0; attempt < MenuExpansionAttemptLimit; attempt++)
        {
            if (await IsExpandedAsync(menu))
                return;
            await Task.Delay(100);
        }
        throw new InvalidOperationException("Navigation submenu did not reach its expanded state.");
    }
}
