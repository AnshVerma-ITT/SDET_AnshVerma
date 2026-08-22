using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class PlaywrightFeatureTests : TestBase
{
    public PlaywrightFeatureTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task XPathAxesAndContains_ShouldLocateInventoryElements()
    {
        await OpenAndLoginAsync();

        var containsLocator = Page.Locator(".inventory_item").Filter(new LocatorFilterOptions { HasText = "Sauce Labs" });
        await Expect(containsLocator.Nth(0)).ToContainTextAsync("Sauce Labs");

        await Expect(Page.Locator("//div[contains(@class,'inventory_item_name')]/parent::a").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item')]//child::button[contains(text(),'Add to cart')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//button[contains(text(),'Add to cart')]/ancestor::div[contains(@class,'inventory_item')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item')]//descendant::div[contains(@class,'inventory_item_name')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item_label')]/following-sibling::div[contains(@class,'pricebar')]").Nth(0)).ToBeVisibleAsync();
    }

    [Test]
    public async Task ExplicitWaitAndMultiplePages_ShouldWork()
    {
        await StepAsync("Open first page", () => Page.GotoAsync("/", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        }));
        await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        var secondPage = await Context.NewPageAsync();
        await StepAsync("Open second page in same browser context", () => secondPage.GotoAsync("/", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        }));

        Assert.That(Context.Pages.Count, Is.EqualTo(2));
        await Expect(secondPage.GetByRole(AriaRole.Button, new() { Name = "Login" })).ToBeVisibleAsync();

        await Page.BringToFrontAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Login" })).ToBeVisibleAsync();
    }
}
