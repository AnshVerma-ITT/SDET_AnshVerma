using Microsoft.Playwright;
using SauceDemo.Playwright.Tests.Configuration;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class LoginPage
{
    private const string UsernameSelector = "#user-name";
    private const string ErrorMessageSelector = "[data-test='error']";

    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public ILocator Username => _page.Locator(UsernameSelector);
    public ILocator Password => _page.GetByPlaceholder("Password");
    public ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    public ILocator ErrorMessage => _page.Locator(ErrorMessageSelector);

    public async Task Open()
    {
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                await _page.GotoAsync(AppRoutes.Root, new PageGotoOptions
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

    public async Task FillCredentials(string username, string password)
    {
        await Username.FillAsync(username);
        await Password.FillAsync(password);
    }

    public Task ClickLogin()
    {
        return LoginButton.ClickAsync();
    }

    public async Task Login(string username, string password)
    {
        await FillCredentials(username, password);
        await ClickLogin();
    }
}
