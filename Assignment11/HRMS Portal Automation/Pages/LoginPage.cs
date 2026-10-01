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
    private const string ValidationMessageScript = "element => element.validationMessage || ''";
    private const string CheckValidityScript = "element => element.checkValidity()";
    private const string ValidationSelector = ".mantine-InputWrapper-error:visible, [data-error='true']:visible";

    public LoginPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public ILocator UsernameField => Page.Locator(UsernameSelector);
    public ILocator PasswordField => Page.Locator(PasswordSelector);
    public ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = LoginButtonName, Exact = true });
    public ILocator DashboardBreadcrumb => Page.Locator(DashboardBreadcrumbSelector).Filter(new() { HasText = DashboardText });
    public bool LastLoginButtonEnabled { get; private set; }
    public bool LastUsernameValid { get; private set; }
    public bool LastPasswordValid { get; private set; }
    public string LastUsernameValidationMessage { get; private set; } = string.Empty;
    public string LastPasswordValidationMessage { get; private set; } = string.Empty;
    public IReadOnlyList<string> LastVisibleValidationMessages { get; private set; } = [];

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

    public async Task SubmitForValidationAsync(string username, string password)
    {
        await UsernameField.FillAsync(username);
        await PasswordField.FillAsync(password);
        LastLoginButtonEnabled = await LoginButton.IsEnabledAsync();
        if (LastLoginButtonEnabled)
            await LoginButton.ClickAsync();

        LastUsernameValid = await UsernameField.EvaluateAsync<bool>(CheckValidityScript);
        LastPasswordValid = await PasswordField.EvaluateAsync<bool>(CheckValidityScript);
        LastUsernameValidationMessage = (await UsernameField.EvaluateAsync<string>(ValidationMessageScript)).Trim();
        LastPasswordValidationMessage = (await PasswordField.EvaluateAsync<string>(ValidationMessageScript)).Trim();
        LastVisibleValidationMessages = (await Page.Locator(ValidationSelector).AllInnerTextsAsync())
            .Select(message => message.Trim()).Where(message => message.Length > 0).ToArray();
    }

    public async Task<bool> IsDisplayedAsync() =>
        await UsernameField.IsVisibleAsync() && await PasswordField.IsVisibleAsync() && await LoginButton.IsVisibleAsync();

    public async Task WaitForDashboardAsync()
    {
        await DashboardBreadcrumb.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Pages.AssertUrlAsync(AppRoutes.Dashboard);
    }

    public async Task<string> GetMessageAsync(string expectedMessage)
    {
        var pattern = string.Join(@"\s+", expectedMessage.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Regex.Escape));
        var message = Page.GetByText(new Regex(pattern, RegexOptions.IgnoreCase));
        await message.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        return Regex.Replace(await message.InnerTextAsync(), @"\s+", " ").Trim();
    }
}
