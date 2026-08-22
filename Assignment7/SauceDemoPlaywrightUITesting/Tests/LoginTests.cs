using System.Text.RegularExpressions;
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

        await StepAsync("Open SauceDemo", loginPage.OpenAsync);
        await StepAsync("Fill valid username and password", () =>
            loginPage.FillCredentialsAsync(LoginTestData.ValidUsername, LoginTestData.ValidPassword));
        await StepAsync("Submit with Enter key", () => loginPage.Password.PressAsync("Enter"));

        await Expect(Page).ToHaveURLAsync(new Regex(".*/inventory.html"));
        await Expect(new InventoryPage(Page).Title).ToHaveTextAsync("Products");
    }

    [TestCase("wrong_user", LoginTestData.ValidPassword, "Username and password do not match")]
    [TestCase(LoginTestData.ValidUsername, "wrong_password", "Username and password do not match")]
    [TestCase("", LoginTestData.ValidPassword, "Username is required")]
    [TestCase(LoginTestData.ValidUsername, "", "Password is required")]
    public async Task Login_WithInvalidCredentials_ShouldShowError(string username, string password, string expectedError)
    {
        var loginPage = new LoginPage(Page);

        await StepAsync("Open SauceDemo", loginPage.OpenAsync);
        await StepAsync("Fill login fields", () => loginPage.FillCredentialsAsync(username, password));
        await Expect(loginPage.Username).ToHaveValueAsync(username);

        await StepAsync("Click Login", loginPage.ClickLoginAsync);

        await Expect(loginPage.ErrorMessage).ToBeVisibleAsync();
        await Expect(loginPage.ErrorMessage).ToContainTextAsync(expectedError);
    }
}
