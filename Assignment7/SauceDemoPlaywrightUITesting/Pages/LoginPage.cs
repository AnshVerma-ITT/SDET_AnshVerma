using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public ILocator Username => _page.Locator("#user-name");
    public ILocator Password => _page.GetByPlaceholder("Password");
    public ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    public ILocator ErrorMessage => _page.Locator("[data-test='error']");

    public async Task OpenAsync()
    {
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                await _page.GotoAsync("/", new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.Commit
                });
                await Username.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible
                });
                return;
            }
            catch (Exception exception) when (attempt < 3 && IsRetryableOpenError(exception))
            {
                await _page.WaitForTimeoutAsync(2000);
            }
        }
    }

    private static bool IsRetryableOpenError(Exception exception)
    {
        return exception is TimeoutException or PlaywrightException;
    }

    public async Task FillCredentialsAsync(string username, string password)
    {
        await Username.FillAsync(username);
        await Password.FillAsync(password);
    }

    public Task ClickLoginAsync()
    {
        return LoginButton.ClickAsync();
    }

    public async Task LoginAsync(string username, string password)
    {
        await FillCredentialsAsync(username, password);
        await ClickLoginAsync();
    }
}
