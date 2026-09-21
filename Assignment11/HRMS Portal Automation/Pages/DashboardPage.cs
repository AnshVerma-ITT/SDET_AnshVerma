using System.Globalization;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class DashboardPage : BasePage
{
    private const string DashboardText = "Dashboard";
    private const string DashboardNavigationSelector = "div.mantine-bfmej7";
    private const string DashboardBreadcrumbSelector = "div.mantine-Breadcrumbs-breadcrumb";
    private const string CalendarSelector = ".mantine-Calendar-calendarBase";
    private const string CalendarHeaderSelector = ".mantine-Calendar-calendarHeaderLevel";
    private const string CalendarControlSelector = ".mantine-Calendar-calendarHeaderControl";
    private const string MonthYearFormat = "MMMM yyyy";

    public DashboardPage(IPage page) : base(page) { }

    public ILocator Calendar => Page.Locator(CalendarSelector);
    public ILocator CalendarHeader => Calendar.Locator(CalendarHeaderSelector);
    public ILocator PreviousMonthButton => Calendar.Locator(CalendarControlSelector).First;
    public ILocator NextMonthButton => Calendar.Locator(CalendarControlSelector).Last;
    public string ExpectedMonthYear => DateTime.Today.ToString(MonthYearFormat, CultureInfo.InvariantCulture);
    public ILocator CurrentDay => Calendar.GetByRole(AriaRole.Button,
        new() { Name = DateTime.Today.Day.ToString(CultureInfo.InvariantCulture), Exact = true });

    private ILocator DashboardNavigation => Page.Locator(DashboardNavigationSelector)
        .Filter(new() { HasText = DashboardText });
    private ILocator DashboardBreadcrumb => Page.Locator(DashboardBreadcrumbSelector)
        .Filter(new() { HasText = DashboardText });

    public async Task NavigateToDashboardAsync()
    {
        await DashboardNavigation.ClickAsync();
        await DashboardBreadcrumb.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task HoverCalendarAsync()
    {
        await Calendar.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Calendar.HoverAsync();
    }

    public Task OpenPreviousMonthAsync() => PreviousMonthButton.ClickAsync();

    public Task OpenNextMonthAsync() => NextMonthButton.ClickAsync();
}
