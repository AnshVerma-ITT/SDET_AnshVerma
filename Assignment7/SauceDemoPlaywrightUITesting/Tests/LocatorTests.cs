using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class LocatorTests : TestBase
{
    public LocatorTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task Inventory_WithXPathLocators_ShouldLocateExpectedElements()
    {
        await LoginFlow.OpenAndLogin(Page, Settings);

        var containsLocator = Page.Locator(".inventory_item")
            .Filter(new LocatorFilterOptions { HasText = ProductTestData.BrandName });
        await Expect(containsLocator.Nth(0)).ToContainTextAsync(ProductTestData.BrandName);

        await Expect(Page.Locator("//div[contains(@class,'inventory_item_name')]/parent::a").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item')]//child::button[contains(text(),'Add to cart')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//button[contains(text(),'Add to cart')]/ancestor::div[contains(@class,'inventory_item')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item')]//descendant::div[contains(@class,'inventory_item_name')]").Nth(0)).ToBeVisibleAsync();
        await Expect(Page.Locator("//div[contains(@class,'inventory_item_label')]/following-sibling::div[contains(@class,'pricebar')]").Nth(0)).ToBeVisibleAsync();
    }
}
