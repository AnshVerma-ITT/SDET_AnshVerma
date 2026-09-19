using HRIntimeAutomation.Models;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class NavigationPage : BasePage
{
    private const string DashboardText = "Dashboard";
    private const string OrganizationText = "Organization";
    private const string LeaveAndAttendanceText = "Leave & Attendance";
    private const string MyProfileText = "My profile";
    private const string EmployeeDirectoryText = "Employee directory";
    private const string AttendanceRecordText = "Attendance record";
    private const string LeavesApplicationText = "Leaves application";
    private const string LeaveEntitlementsText = "Leave entitlements";
    private const string LeaveCorrectionText = "Leave correction";
    private const string MyHolidaysText = "My holidays";
    private const string DashboardSelector = "div.mantine-bfmej7";
    private const string SiblingMenuSelector = "xpath=following-sibling::div[1]";
    private const string MenuLinkSelector = "a";
    private const string BreadcrumbSelector = "a.mantine-Text-root.mantine-Anchor-root";
    private const string DashboardBreadcrumbSelector = "div.mantine-Breadcrumbs-breadcrumb";
    private const string AriaHiddenAttribute = "aria-hidden";
    private const string ExpandedValue = "false";
    private const string MenuExpansionFailureMessage = "Navigation submenu did not reach its expanded state.";
    private const int MenuExpansionAttemptLimit = 20;
    private const int MenuExpansionPollMilliseconds = 100;

    public NavigationPage(IPage page) : base(page) { }

    private ILocator Dashboard => Page.Locator(DashboardSelector).Filter(new() { HasText = DashboardText });
    private ILocator OrganizationButton => Page.GetByRole(AriaRole.Button)
        .Filter(new() { HasText = OrganizationText });
    private ILocator OrganizationMenu => OrganizationButton.Locator(SiblingMenuSelector);
    private ILocator LeaveAndAttendanceButton => Page.GetByRole(AriaRole.Button)
        .Filter(new() { HasText = LeaveAndAttendanceText });
    private ILocator LeaveAndAttendanceMenu => LeaveAndAttendanceButton.Locator(SiblingMenuSelector);
    private ILocator MyProfile => OrganizationMenu.Locator(MenuLinkSelector).Filter(new() { HasText = MyProfileText });
    private ILocator EmployeeDirectory => OrganizationMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = EmployeeDirectoryText });
    private ILocator AttendanceRecord => LeaveAndAttendanceMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = AttendanceRecordText });
    private ILocator LeavesApplication => LeaveAndAttendanceMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = LeavesApplicationText });
    private ILocator LeaveEntitlements => LeaveAndAttendanceMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = LeaveEntitlementsText });
    private ILocator LeaveCorrection => LeaveAndAttendanceMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = LeaveCorrectionText });
    private ILocator MyHolidays => LeaveAndAttendanceMenu.Locator(MenuLinkSelector)
        .Filter(new() { HasText = MyHolidaysText });
    private ILocator DashboardBreadcrumb => Page.Locator(DashboardBreadcrumbSelector)
        .Filter(new() { HasText = DashboardText });

    public async Task OpenOrganizationAsync()
    {
        if (!await IsExpandedAsync(OrganizationMenu))
            await OrganizationButton.ClickAsync();

        await WaitForExpandedMenuAsync(OrganizationMenu);
        await MyProfile.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await EmployeeDirectory.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task OpenLeaveAndAttendanceAsync()
    {
        if (!await IsExpandedAsync(LeaveAndAttendanceMenu))
            await LeaveAndAttendanceButton.ClickAsync();

        await WaitForExpandedMenuAsync(LeaveAndAttendanceMenu);
        await AttendanceRecord.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await LeavesApplication.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await LeaveEntitlements.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await LeaveCorrection.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await MyHolidays.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public Task<NavigationResult> OpenDashboardAsync() =>
        OpenDestinationAsync(DashboardText, Dashboard, DashboardBreadcrumb, requiresUrlChange: false);
    public Task<NavigationResult> OpenMyProfileAsync() =>
        OpenDestinationAsync(MyProfileText, MyProfile, Breadcrumb(MyProfileText));
    public Task<NavigationResult> OpenEmployeeDirectoryAsync() =>
        OpenDestinationAsync(EmployeeDirectoryText, EmployeeDirectory, Breadcrumb(EmployeeDirectoryText));
    public Task<NavigationResult> OpenAttendanceRecordAsync() =>
        OpenDestinationAsync(AttendanceRecordText, AttendanceRecord, Breadcrumb(AttendanceRecordText));
    public Task<NavigationResult> OpenLeavesApplicationAsync() =>
        OpenDestinationAsync(LeavesApplicationText, LeavesApplication, Breadcrumb(LeavesApplicationText));
    public Task<NavigationResult> OpenLeaveEntitlementsAsync() =>
        OpenDestinationAsync(LeaveEntitlementsText, LeaveEntitlements, Breadcrumb(LeaveEntitlementsText));
    public Task<NavigationResult> OpenLeaveCorrectionAsync() =>
        OpenDestinationAsync(LeaveCorrectionText, LeaveCorrection, Breadcrumb(LeaveCorrectionText));
    public Task<NavigationResult> OpenMyHolidaysAsync() =>
        OpenDestinationAsync(MyHolidaysText, MyHolidays, Breadcrumb(MyHolidaysText));

    private ILocator Breadcrumb(string text) => Page.Locator(BreadcrumbSelector)
        .Filter(new() { HasText = text });

    private static async Task<bool> IsExpandedAsync(ILocator menu) =>
        string.Equals(await menu.GetAttributeAsync(AriaHiddenAttribute), ExpandedValue, StringComparison.OrdinalIgnoreCase)
        && await menu.IsVisibleAsync();

    private static async Task WaitForExpandedMenuAsync(ILocator menu)
    {
        await menu.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        for (var attempt = 0; attempt < MenuExpansionAttemptLimit; attempt++)
        {
            if (string.Equals(await menu.GetAttributeAsync(AriaHiddenAttribute), ExpandedValue,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await Task.Delay(MenuExpansionPollMilliseconds);
        }

        throw new InvalidOperationException(MenuExpansionFailureMessage);
    }

    private async Task<NavigationResult> OpenDestinationAsync(
        string destination,
        ILocator link,
        ILocator destinationMarker,
        bool requiresUrlChange = true)
    {
        var previousUrl = Page.Url;
        await link.ClickAsync();
        await destinationMarker.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        return new NavigationResult(
            destination,
            previousUrl,
            Page.Url,
            await destinationMarker.IsVisibleAsync(),
            requiresUrlChange);
    }
}
