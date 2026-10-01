using Microsoft.Playwright;

namespace HRIntimeAutomation.Wrappers;

public sealed class ElementActions
{
    public Task ClickAsync(ILocator locator) => Required(locator).ClickAsync();
    public Task FillAsync(ILocator locator, string value) => Required(locator).FillAsync(value);
    public Task HoverAsync(ILocator locator) => Required(locator).HoverAsync();
    public Task<string> GetTextAsync(ILocator locator) => Required(locator).InnerTextAsync();
    public Task<bool> IsVisibleAsync(ILocator locator) => Required(locator).IsVisibleAsync();
    public Task<string?> GetAttributeAsync(ILocator locator, string name) =>
        Required(locator).GetAttributeAsync(name);
    public Task WaitUntilVisibleAsync(ILocator locator) =>
        Required(locator).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    public Task WaitUntilHiddenAsync(ILocator locator) =>
        Required(locator).WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    public Task WaitForAttributeAsync(ILocator locator, string name, string value) =>
        Microsoft.Playwright.Assertions.Expect(Required(locator)).ToHaveAttributeAsync(name, value);

    private static ILocator Required(ILocator locator)
    {
        ArgumentNullException.ThrowIfNull(locator);
        return locator;
    }
}
