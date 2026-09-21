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
    private const string AriaCurrentAttribute = "aria-current";
    private const string DataActiveAttribute = "data-active";
    private const string ClassAttribute = "class";
    private const string HrefAttribute = "href";
    private const string ExpandedValue = "false";
    private const string ActiveValue = "true";
    private const string CurrentPageValue = "page";
    private const string ActiveClassToken = "active";
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

    public async Task<NavigationSectionResult> CollapseAndReopenOrganizationAsync()
    {
        await OpenOrganizationAsync();
        await OrganizationButton.ClickAsync();
        await OrganizationMenu.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        var wasCollapsed = !await IsExpandedAsync(OrganizationMenu);

        await OpenOrganizationAsync();
        return new NavigationSectionResult(wasCollapsed, await IsExpandedAsync(OrganizationMenu));
    }

    public Task ReloadAsync() => Page.ReloadAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });

    public Task<NavigationResult> OpenDashboardAsync() =>
        OpenDestinationAsync(
            DashboardText,
            Dashboard,
            DashboardBreadcrumb,
            requiresUrlChange: false,
            expectedUrlPathOverride: Configuration.AppRoutes.Dashboard);
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
        bool requiresUrlChange = true,
        string? expectedUrlPathOverride = null)
    {
        var previousUrl = Page.Url;
        var linkHref = await link.GetAttributeAsync(HrefAttribute);
        var expectedUrlPath = expectedUrlPathOverride ?? GetExpectedPath(previousUrl, linkHref);
        await link.ClickAsync();
        await destinationMarker.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var currentUrl = Page.Url;
        if (expectedUrlPath is null && Uri.TryCreate(currentUrl, UriKind.Absolute, out var reachedUri))
            expectedUrlPath = reachedUri.AbsolutePath;

        var ariaCurrent = await link.GetAttributeAsync(AriaCurrentAttribute);
        var dataActive = await link.GetAttributeAsync(DataActiveAttribute);
        var className = await link.GetAttributeAsync(ClassAttribute);
        var urlMatches = expectedUrlPath is not null
            && Uri.TryCreate(currentUrl, UriKind.Absolute, out var currentUri)
            && string.Equals(currentUri.AbsolutePath.TrimEnd('/'), expectedUrlPath.TrimEnd('/'),
                StringComparison.OrdinalIgnoreCase);
        var destinationIsActive = urlMatches
            || string.Equals(ariaCurrent, CurrentPageValue, StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataActive, ActiveValue, StringComparison.OrdinalIgnoreCase)
            || (className?.Contains(ActiveClassToken, StringComparison.OrdinalIgnoreCase) ?? false);

        return new NavigationResult(
            destination,
            linkHref,
            expectedUrlPath,
            previousUrl,
            currentUrl,
            await Page.TitleAsync(),
            (await destinationMarker.InnerTextAsync()).Trim(),
            await destinationMarker.IsVisibleAsync(),
            destinationIsActive,
            requiresUrlChange);
    }

    private static string? GetExpectedPath(string currentUrl, string? linkHref)
    {
        if (string.IsNullOrWhiteSpace(linkHref)
            || !Uri.TryCreate(currentUrl, UriKind.Absolute, out var baseUri)
            || !Uri.TryCreate(baseUri, linkHref, out var destinationUri))
        {
            return null;
        }

        return destinationUri.AbsolutePath;
    }
}
