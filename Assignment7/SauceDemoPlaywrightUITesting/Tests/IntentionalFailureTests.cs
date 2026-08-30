using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.TestData;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
[Category("FailureDemo")]
public sealed class IntentionalFailureTests : TestBase
{
    public IntentionalFailureTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task Inventory_WithIncorrectExpectedHeading_ShouldFailIntentionally()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);
        var actualHeading = await inventoryPage.GetPageTitle();

        Assert.That(actualHeading, Is.EqualTo(PageTitleTestData.IntentionalIncorrectInventory));
    }
}
