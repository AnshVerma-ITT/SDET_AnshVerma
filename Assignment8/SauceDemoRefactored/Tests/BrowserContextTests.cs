using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Infrastructure;
using SauceDemo.Playwright.Tests.Pages;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class BrowserContextTests : TestBase
{
    public BrowserContextTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task BrowserContext_WithMultiplePages_ShouldSupportExplicitWait()
    {
        var firstLoginPage = new LoginPage(Page);
        await AllureHelper.Step("Open first page", async () =>
        {
            await NavigationHelper.NavigateTo(Page, AppRoutes.Root, Settings);
            await firstLoginPage.WaitUntilLoaded();
        });

        var secondPage = await Context.NewPageAsync();
        var secondLoginPage = new LoginPage(secondPage);
        await AllureHelper.Step("Open second page in same browser context", async () =>
        {
            await NavigationHelper.NavigateTo(secondPage, AppRoutes.Root, Settings);
            await secondLoginPage.WaitUntilLoaded();
        });

        Assert.That(Context.Pages.Count, Is.EqualTo(2));
        Assert.That(await secondLoginPage.IsLoginButtonDisplayed(), Is.True);

        await Page.BringToFrontAsync();
        Assert.That(await firstLoginPage.IsLoginButtonDisplayed(), Is.True);
    }
}
