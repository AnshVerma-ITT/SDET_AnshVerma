using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
[Category("FailureDemo")]
public sealed class IntentionalFailureTests : TestBase
{
    public IntentionalFailureTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task ProductHeading_ShouldDemonstrateARealUiFailure()
    {
        var inventoryPage = await OpenAndLoginAsync();
        var actualHeading = await inventoryPage.Title.InnerTextAsync();

        Assert.That(actualHeading, Is.EqualTo("Product Catalog"));
    }
}
