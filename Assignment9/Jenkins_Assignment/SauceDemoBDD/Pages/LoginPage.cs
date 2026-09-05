using Microsoft.Playwright;
using SauceDemoBDD.Configuration;

namespace SauceDemoBDD.Pages;

public sealed class LoginPage
{
    private const string UsernameTestId = "username";
    private const string PasswordTestId = "password";
    private const string LoginButtonTestId = "login-button";
    private const string ErrorTestId = "error";

    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    private ILocator Username => _page.GetByTestId(UsernameTestId);
    private ILocator Password => _page.GetByTestId(PasswordTestId);
    private ILocator LoginButton => _page.GetByTestId(LoginButtonTestId);
    private ILocator ErrorMessage => _page.GetByTestId(ErrorTestId);

    public async Task Open()
    {
        await _page.GotoAsync(AppRoutes.Login);
        await LoginButton.WaitForAsync();
    }

    public async Task Login(string username, string password)
    {
        await Username.FillAsync(username);
        await Password.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public Task<string> GetErrorMessage()
    {
        return ErrorMessage.InnerTextAsync();
    }
}
