using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
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
}
