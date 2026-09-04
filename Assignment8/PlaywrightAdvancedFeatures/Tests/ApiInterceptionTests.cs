using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightAdvancedFeatures.Configuration;
using PlaywrightAdvancedFeatures.Fixtures;
using PlaywrightAdvancedFeatures.Locators;
using PlaywrightAdvancedFeatures.TestData;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightAdvancedFeatures.Tests;

[Category(TestCategories.AdvancedFeatures)]
public sealed class ApiInterceptionTests : BrowserTestBase
{
    [Test]
    public async Task Network_InterceptApiCall_ShouldReturnMockedProduct()
    {
        var requestWasIntercepted = false;
        await Page.RouteAsync(ApiInterceptionTestData.RoutePattern, async route =>
        {
            requestWasIntercepted = true;
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = ApiInterceptionTestData.SuccessStatusCode,
                ContentType = ApiInterceptionTestData.JsonContentType,
                Headers = new Dictionary<string, string>
                {
                    [ApiInterceptionTestData.CorsHeaderName] = ApiInterceptionTestData.CorsHeaderValue
                },
                Body = ApiInterceptionTestData.ResponseBody
            });
        });

        await Page.SetContentAsync(ApiInterceptionTestData.PageContent);
        await Page.Locator(AdvancedFeatureLocators.LoadProductButton).ClickAsync();

        await Expect(Page.Locator(AdvancedFeatureLocators.ProductName))
            .ToHaveTextAsync(ApiInterceptionTestData.MockProductName);
        Assert.That(requestWasIntercepted, Is.True);
    }
}
