using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class PlaywrightFeatureTests : TestBase
{
    public PlaywrightFeatureTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task Inventory_WithXPathLocators_ShouldLocateExpectedElements()
    {
        await OpenAndLogin();

        var containsLocator = Page.Locator(".inventory_item")
            .Filter(new LocatorFilterOptions { HasText = ProductTestData.BrandName });
        await Expect(containsLocator.Nth(0)).ToContainTextAsync(ProductTestData.BrandName);

        await Expect(Page.Locator("//div[contains(@class,'inventory_item_name')]/parent::a").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item')]//child::button[contains(text(),'Add to cart')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//button[contains(text(),'Add to cart')]/ancestor::div[contains(@class,'inventory_item')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item')]//descendant::div[contains(@class,'inventory_item_name')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item_label')]/following-sibling::div[contains(@class,'pricebar')]").Nth(0)).ToBeVisibleAsync();
    }

    [Test]
    public async Task BrowserContext_WithMultiplePages_ShouldSupportExplicitWait()
    {
        var firstLoginPage = new LoginPage(Page);
        await StepAsync("Open first page", () => Page.GotoAsync(AppRoutes.Root, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        }));
        await firstLoginPage.LoginButton.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        var secondPage = await Context.NewPageAsync();
        var secondLoginPage = new LoginPage(secondPage);
        await StepAsync("Open second page in same browser context", () => secondPage.GotoAsync(AppRoutes.Root, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        }));

        Assert.That(Context.Pages.Count, Is.EqualTo(2));
        await Expect(secondLoginPage.LoginButton).ToBeVisibleAsync();

        await Page.BringToFrontAsync();
        await Expect(firstLoginPage.LoginButton).ToBeVisibleAsync();
    }
}
