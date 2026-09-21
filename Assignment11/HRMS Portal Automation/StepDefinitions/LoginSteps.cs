using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.Models;
using HRIntimeAutomation.TestData;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class LoginSteps
{
    private readonly ScenarioTestContext _context;

    public LoginSteps(ScenarioTestContext context) => _context = context;

    [Given("I open the HRMS login page")]
    public Task GivenIOpenTheHrmsLoginPage() => _context.LoginPage.OpenAsync();

    [Then("the login form should be displayed")]
    public async Task ThenTheLoginFormShouldBeDisplayed() =>
        Assert.That(await _context.LoginPage.IsDisplayedAsync(), Is.True,
            "Login form was not displayed.");

    [When("I login with valid credentials")]
    public async Task WhenILoginWithValidCredentials()
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        await _context.LoginPage.LoginAsync(credentials.Username, credentials.Password);
    }

    [When("I login with invalid credentials")]
    public Task WhenILoginWithInvalidCredentials() =>
        _context.LoginPage.LoginAsync(LoginTestData.InvalidUsername, LoginTestData.InvalidPassword);

    [When("I submit login using {string} username and {string} password")]
    public async Task WhenISubmitLoginUsingUsernameAndPassword(string usernameToken, string passwordToken)
    {
        var needsValidCredentials = RequiresValidCredential(usernameToken)
            || RequiresValidCredential(passwordToken);
        var credentials = needsValidCredentials
            ? CredentialProvider.GetRequiredAdminCredentials()
            : new Credentials(string.Empty, string.Empty);
        var username = ResolveCredential(usernameToken, credentials.Username, LoginTestData.InvalidUsername);
        var password = ResolveCredential(passwordToken, credentials.Password, LoginTestData.InvalidPassword);

        _context.LoginValidationResult = await _context.LoginPage
            .SubmitForValidationAsync(username, password);
    }

    [Then("the Dashboard should be opened")]
    public async Task ThenTheDashboardShouldBeOpened()
    {
        await _context.LoginPage.WaitForDashboardAsync();
        Assert.That(_context.Driver.Page.Url, Does.Contain(LoginTestData.DashboardUrlPath));
    }

    [Then("the invalid login error should be displayed")]
    public async Task ThenTheInvalidLoginErrorShouldBeDisplayed()
    {
        var actualMessage = await _context.LoginPage.GetMessageAsync(
            LoginTestData.ExpectedInvalidLoginMessage);
        Assert.That(actualMessage, Is.EqualTo(LoginTestData.ExpectedInvalidLoginMessage));
    }

    [Then("the login result should be {string}")]
    public async Task ThenTheLoginResultShouldBe(string expectedResult)
    {
        Assert.That(_context.LoginValidationResult, Is.Not.Null,
            "The login attempt result was not captured.");

        switch (expectedResult)
        {
            case LoginTestData.DashboardResult:
                await ThenTheDashboardShouldBeOpened();
                break;

            case LoginTestData.ErrorResult:
                await AssertRejectedAuthenticationAsync();
                break;

            case LoginTestData.ValidationResult:
                AssertValidationPreventedAuthentication(_context.LoginValidationResult!);
                break;

            default:
                Assert.Fail($"Unsupported expected login result: {expectedResult}");
                break;
        }
    }


    private async Task AssertRejectedAuthenticationAsync()
    {
        var page = _context.Driver.Page;
        await page.WaitForTimeoutAsync(1000);

        Assert.Multiple(() =>
        {
            Assert.That(_context.LoginValidationResult!.DashboardVisible, Is.False,
                "Rejected credentials must not open the Dashboard.");
            Assert.That(GetPath(page.Url), Is.Not.EqualTo(LoginTestData.DashboardUrlPath).IgnoreCase,
                "Rejected credentials must not navigate to the Dashboard path.");
        });

        Assert.That(await _context.LoginPage.IsDisplayedAsync(), Is.True,
            "The login form should remain displayed after rejected credentials.");
    }

    private static string GetPath(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.AbsolutePath : url;

    private static string ResolveCredential(string token, string validValue, string invalidValue) =>
        token switch
        {
            LoginTestData.ValidCredentialToken => validValue,
            LoginTestData.InvalidCredentialToken => invalidValue,
            LoginTestData.BlankCredentialToken => string.Empty,
            LoginTestData.ValidWithSpacesCredentialToken => $"  {validValue}  ",
            _ => throw new ArgumentOutOfRangeException(nameof(token), token, "Unsupported credential token.")
        };

    private static bool RequiresValidCredential(string token) =>
        token is LoginTestData.ValidCredentialToken or LoginTestData.ValidWithSpacesCredentialToken;

    private static void AssertValidationPreventedAuthentication(LoginValidationResult result)
    {
        var validationWasExposed = !result.LoginButtonEnabled
            || !result.UsernameValid
            || !result.PasswordValid
            || !string.IsNullOrWhiteSpace(result.UsernameValidationMessage)
            || !string.IsNullOrWhiteSpace(result.PasswordValidationMessage)
            || result.VisibleValidationMessages.Count > 0;

        Assert.Multiple(() =>
        {
            Assert.That(result.DashboardVisible, Is.False,
                "Incomplete credentials must not open the Dashboard.");
            Assert.That(result.LoginFormVisible, Is.True,
                "The login form should remain displayed for incomplete credentials.");
            Assert.That(validationWasExposed, Is.True,
                "The form neither disabled submission nor exposed field validation for incomplete credentials.");
        });
    }
}
