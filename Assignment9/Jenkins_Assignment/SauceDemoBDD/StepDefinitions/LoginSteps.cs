using NUnit.Framework;
using Reqnroll;
using SauceDemoBDD.Configuration;
using SauceDemoBDD.Flows;
using SauceDemoBDD.Support;
using SauceDemoBDD.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemoBDD.StepDefinitions;

[Binding]
public sealed class LoginSteps
{
    private readonly BrowserDriver _driver;
    private readonly ScenarioState _state;

    public LoginSteps(BrowserDriver driver, ScenarioState state)
    {
        _driver = driver;
        _state = state;
    }

    [Given("I am on the SauceDemo login page")]
    public Task GivenIAmOnTheLoginPage()
    {
        return _driver.LoginPage.Open();
    }

    [Given("I am logged in to SauceDemo")]
    public Task GivenIAmLoggedIn()
    {
        return new LoginFlow(_driver.LoginPage).LoginAsStandardUser();
    }

    [When("I login with valid credentials")]
    public Task WhenILoginWithValidCredentials()
    {
        return _driver.LoginPage.Login(
            LoginTestData.StandardUsername,
            LoginTestData.StandardPassword);
    }

    [When("I login using {string} credentials")]
    public async Task WhenILoginUsingCredentials(string caseName)
    {
        var loginCase = LoginTestData.GetCase(caseName);
        _state.ExpectedLoginError = loginCase.ExpectedError;
        await _driver.LoginPage.Login(loginCase.Username, loginCase.Password);
    }

    [Then("the inventory page should be displayed")]
    public async Task ThenTheInventoryPageShouldBeDisplayed()
    {
        await Expect(_driver.Page).ToHaveURLAsync(AppRoutes.Inventory);
        Assert.That(await _driver.InventoryPage.IsDisplayed(), Is.True);
    }

    [Then("login should be rejected")]
    public async Task ThenLoginShouldBeRejected()
    {
        Assert.That(
            await _driver.LoginPage.GetErrorMessage(),
            Does.Contain(_state.ExpectedLoginError));
    }
}
