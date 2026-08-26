using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class LoginTests : TestBase
{
    public LoginTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task Login_WithValidCredentials_ShouldSucceed()
    {
        var loginPage = new LoginPage(Page);

        await StepAsync("Open SauceDemo", loginPage.Open);
        await StepAsync("Fill valid username and password", () =>
            loginPage.FillCredentials(LoginTestData.ValidUsername, LoginTestData.ValidPassword));
        await StepAsync("Submit with Enter key", () => loginPage.Password.PressAsync("Enter"));

        await Expect(Page).ToHaveURLAsync(AppRoutes.InventoryUrlPattern);
        await Expect(new InventoryPage(Page).Title).ToHaveTextAsync(ExpectedText.InventoryTitle);
    }

    [TestCase(LoginTestData.InvalidUsername, LoginTestData.ValidPassword, ExpectedText.LoginMismatchError)]
    [TestCase(LoginTestData.ValidUsername, LoginTestData.InvalidPassword, ExpectedText.LoginMismatchError)]
    [TestCase("", LoginTestData.ValidPassword, ExpectedText.UsernameRequiredError)]
    [TestCase(LoginTestData.ValidUsername, "", ExpectedText.PasswordRequiredError)]
    public async Task Login_WithInvalidCredentials_ShouldShowError(string username, string password, string expectedError)
    {
        var loginPage = new LoginPage(Page);

        await StepAsync("Open SauceDemo", loginPage.Open);
        await StepAsync("Fill login fields", () => loginPage.FillCredentials(username, password));
        await Expect(loginPage.Username).ToHaveValueAsync(username);

        await StepAsync("Click Login", loginPage.ClickLogin);

        await Expect(loginPage.ErrorMessage).ToBeVisibleAsync();
        await Expect(loginPage.ErrorMessage).ToContainTextAsync(expectedError);
    }
}
