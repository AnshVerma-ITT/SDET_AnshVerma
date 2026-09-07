using NUnit.Framework;
using PlaywrightAdvancedFeatures.Configuration;
using PlaywrightAdvancedFeatures.Fixtures;
using PlaywrightAdvancedFeatures.Locators;
using PlaywrightAdvancedFeatures.TestData;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightAdvancedFeatures.Tests;

[Category(TestCategories.AdvancedFeatures)]
public sealed class IFrameTests : BrowserTestBase
{
    [Test]
    public async Task IFrame_InteractUsingFrameLocator_ShouldSucceed()
    {
        await Page.SetContentAsync(IFrameTestData.PageContent);

        var frame = Page.FrameLocator(AdvancedFeatureLocators.Frame);
        await frame.Locator(AdvancedFeatureLocators.FrameButton).ClickAsync();
        await Expect(frame.Locator(AdvancedFeatureLocators.FrameMessage))
            .ToHaveTextAsync(IFrameTestData.UpdatedMessage);
    }
}
