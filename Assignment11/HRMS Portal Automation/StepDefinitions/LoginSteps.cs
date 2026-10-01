using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class LoginSteps
{
    private readonly ScenarioTestContext _context;

    public LoginSteps(ScenarioTestContext context) => _context = context;

    [Given("I open the HRMS login page")]
    public Task OpenLoginPage() => _context.LoginPage.OpenAsync();

    [Then("the login form should be displayed")]
    public async Task LoginFormIsDisplayed() =>
        Assert.That(await _context.LoginPage.IsDisplayedAsync(), Is.True, "Login form was not displayed.");

    [When("I login with valid credentials")]
    public async Task LoginWithValidCredentials()
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        await _context.LoginPage.LoginAsync(credentials.Username, credentials.Password);
    }

    [When("I submit the configured username with password {string}")]
    public async Task SubmitConfiguredUsernameWithPassword(string password)
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        await _context.LoginPage.SubmitForValidationAsync(credentials.Username, password);
    }

    [When("I submit username {string} with the configured password")]
    public async Task SubmitUsernameWithConfiguredPassword(string username)
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        await _context.LoginPage.SubmitForValidationAsync(username, credentials.Password);
    }

    [When("I submit username {string} and password {string}")]
    public Task SubmitCredentials(string username, string password) =>
        _context.LoginPage.SubmitForValidationAsync(username, password);

    [When("I submit the login form without the {string}")]
    public async Task SubmitWithoutField(string field)
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        var username = field is "username" or "both" ? string.Empty : credentials.Username;
        var password = field is "password" or "both" ? string.Empty : credentials.Password;
        await _context.LoginPage.SubmitForValidationAsync(username, password);
    }

    [When("I add leading and trailing spaces to the configured {string}")]
    public async Task AddWhitespaceToConfiguredCredential(string field)
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        var username = field == "username" ? $"  {credentials.Username}  " : credentials.Username;
        var password = field == "password" ? $"  {credentials.Password}  " : credentials.Password;
        await _context.LoginPage.SubmitForValidationAsync(username, password);
    }

    [Then("the Dashboard should be opened")]
    public Task DashboardIsOpened() => _context.LoginPage.WaitForDashboardAsync();

    [Then("authentication should be rejected")]
    public async Task AuthenticationIsRejected()
    {
        await _context.Driver!.Page.WaitForTimeoutAsync(1000);
        var loginFormIsVisible = await _context.LoginPage.IsDisplayedAsync();
        Assert.Multiple(() =>
        {
            Assert.That(_context.LoginPage.Pages.RouteMatches(_context.Driver.Page.Url, AppRoutes.Dashboard), Is.False);
            Assert.That(loginFormIsVisible, Is.True);
        });
        await _context.LoginPage.Pages.AssertUrlAsync(AppRoutes.Login);
    }

    [Then("login validation should prevent authentication")]
    public async Task ValidationPreventsAuthentication()
    {
        var page = _context.LoginPage;
        var validationShown = !page.LastLoginButtonEnabled || !page.LastUsernameValid || !page.LastPasswordValid
            || page.LastUsernameValidationMessage.Length > 0 || page.LastPasswordValidationMessage.Length > 0
            || page.LastVisibleValidationMessages.Count > 0;
        var loginFormIsVisible = await page.IsDisplayedAsync();
        Assert.Multiple(() =>
        {
            Assert.That(validationShown, Is.True, "No validation was exposed for incomplete credentials.");
            Assert.That(loginFormIsVisible, Is.True);
        });
        await page.Pages.AssertUrlAsync(AppRoutes.Login);
    }
}
