using System.Text.RegularExpressions;
using HRIntimeAutomation.Configuration;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class LoginPage : BasePage
{
    private const string UsernameSelector = "#login-form-input-username";
    private const string PasswordSelector = "#login-form-input-password";
    private const string LoginButtonName = "Login";
    private const string DashboardText = "Dashboard";
    private const string DashboardBreadcrumbSelector = "div.mantine-Breadcrumbs-breadcrumb";
    private const string FlexibleWhitespacePattern = @"\s+";
    private const string SingleSpace = " ";

    public LoginPage(IPage page) : base(page) { }

    private ILocator UsernameField => Page.Locator(UsernameSelector);
    private ILocator PasswordField => Page.Locator(PasswordSelector);
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button,
        new() { Name = LoginButtonName, Exact = true });
    private ILocator DashboardBreadcrumb => Page.Locator(DashboardBreadcrumbSelector)
        .Filter(new() { HasText = DashboardText });

    public async Task OpenAsync()
    {
        await NavigateAsync(AppRoutes.Login);
        await UsernameField.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await PasswordField.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await LoginButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameField.FillAsync(username);
        await PasswordField.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<bool> IsDisplayedAsync() =>
        await UsernameField.IsVisibleAsync()
        && await PasswordField.IsVisibleAsync()
        && await LoginButton.IsVisibleAsync();

    public Task WaitForDashboardAsync() =>
        DashboardBreadcrumb.WaitForAsync(new() { State = WaitForSelectorState.Visible });

    public async Task<string> GetMessageAsync(string expectedMessage)
    {
        var messagePattern = string.Join(
            FlexibleWhitespacePattern,
            expectedMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Regex.Escape));
        var message = Page.GetByText(new Regex(messagePattern, RegexOptions.IgnoreCase));
        await message.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        return Regex.Replace(await message.InnerTextAsync(), FlexibleWhitespacePattern, SingleSpace).Trim();
    }
}
