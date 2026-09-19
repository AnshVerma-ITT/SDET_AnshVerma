using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public abstract class BasePage
{
    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected IPage Page { get; }

    protected Task NavigateAsync(string route) =>
        Page.GotoAsync(route, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

    protected Task ClickAsync(ILocator locator) => locator.ClickAsync();

    protected Task FillAsync(ILocator locator, string value) => locator.FillAsync(value);

    protected Task HoverAsync(ILocator locator) => locator.HoverAsync();

    protected Task<string> TextAsync(ILocator locator) => locator.InnerTextAsync();

    protected Task<bool> IsVisibleAsync(ILocator locator) => locator.IsVisibleAsync();

    protected Task WaitUntilVisibleAsync(ILocator locator) =>
        locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
}
