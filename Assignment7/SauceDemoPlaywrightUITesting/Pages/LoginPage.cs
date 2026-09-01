using Microsoft.Playwright;

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

    private ILocator Username => _page.Locator(UsernameSelector);
    private ILocator Password => _page.GetByPlaceholder("Password");
    private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator ErrorMessage => _page.Locator(ErrorMessageSelector);

    public Task WaitUntilLoaded()
    {
        return Username.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });
    }

    public async Task FillCredentials(string username, string password)
    {
        await Username.FillAsync(username);
        await Password.FillAsync(password);
    }

    public Task ClickOnLoginButton()
    {
        return LoginButton.ClickAsync();
    }

    public async Task Login(string username, string password)
    {
        await FillCredentials(username, password);
        await ClickOnLoginButton();
    }

    public Task<string> GetUsernameValue()
    {
        return Username.InputValueAsync();
    }

    public Task<bool> IsLoginButtonDisplayed()
    {
        return LoginButton.IsVisibleAsync();
    }

    public Task<bool> IsErrorDisplayed()
    {
        return ErrorMessage.IsVisibleAsync();
    }

    public Task<string> GetErrorMessage()
    {
        return ErrorMessage.InnerTextAsync();
    }
}
