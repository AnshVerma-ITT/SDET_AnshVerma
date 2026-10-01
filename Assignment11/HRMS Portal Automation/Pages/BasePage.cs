using HRIntimeAutomation.Wrappers;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public abstract class BasePage
{
    protected BasePage(IPage page, string baseUrl)
    {
        Page = page;
        Elements = new ElementActions();
        Pages = new PageActions(page, baseUrl);
    }

    protected IPage Page { get; }
    protected ElementActions Elements { get; }
    public PageActions Pages { get; }

    protected Task NavigateAsync(string route) => Pages.OpenAsync(route);

    protected Task ClickAsync(ILocator locator) => Elements.ClickAsync(locator);

    protected Task FillAsync(ILocator locator, string value) => Elements.FillAsync(locator, value);

    protected Task HoverAsync(ILocator locator) => Elements.HoverAsync(locator);

    protected Task<string> TextAsync(ILocator locator) => Elements.GetTextAsync(locator);

    protected Task<bool> IsVisibleAsync(ILocator locator) => Elements.IsVisibleAsync(locator);

    protected Task WaitUntilVisibleAsync(ILocator locator) =>
        Elements.WaitUntilVisibleAsync(locator);
}
