using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Infrastructure;
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

        await AllureHelper.Step("Open SauceDemo", async () =>
        {
            await NavigationHelper.NavigateTo(Page, AppRoutes.Root, Settings);
            await loginPage.WaitUntilLoaded();
        });
        await AllureHelper.Step("Fill valid username and password", () =>
            loginPage.FillCredentials(LoginTestData.ValidUsername, LoginTestData.ValidPassword));
        await AllureHelper.Step("Click login button", loginPage.ClickOnLoginButton);

        await Expect(Page).ToHaveURLAsync(AppRoutes.Inventory);
        Assert.That(await new InventoryPage(Page).GetPageTitle(), Is.EqualTo(PageTitleTestData.Inventory));
    }

    [TestCase(LoginTestData.InvalidUsername, LoginTestData.ValidPassword, AppErrorTestData.LoginMismatch)]
    [TestCase(LoginTestData.ValidUsername, LoginTestData.InvalidPassword, AppErrorTestData.LoginMismatch)]
    [TestCase("", LoginTestData.ValidPassword, AppErrorTestData.UsernameRequired)]
    [TestCase(LoginTestData.ValidUsername, "", AppErrorTestData.PasswordRequired)]
    public async Task Login_WithInvalidCredentials_ShouldShowError(string username, string password, string expectedError)
    {
        var loginPage = new LoginPage(Page);

        await AllureHelper.Step("Open SauceDemo", async () =>
        {
            await NavigationHelper.NavigateTo(Page, AppRoutes.Root, Settings);
            await loginPage.WaitUntilLoaded();
        });
        await AllureHelper.Step("Fill login fields", () => loginPage.FillCredentials(username, password));
        Assert.That(await loginPage.GetUsernameValue(), Is.EqualTo(username));

        await AllureHelper.Step("Click Login", loginPage.ClickOnLoginButton);

        Assert.That(await loginPage.IsErrorDisplayed(), Is.True);
        Assert.That(await loginPage.GetErrorMessage(), Does.Contain(expectedError));
    }
}
