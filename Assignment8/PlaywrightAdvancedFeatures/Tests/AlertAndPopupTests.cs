using NUnit.Framework;
using PlaywrightAdvancedFeatures.Configuration;
using PlaywrightAdvancedFeatures.Fixtures;
using PlaywrightAdvancedFeatures.Locators;
using PlaywrightAdvancedFeatures.TestData;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightAdvancedFeatures.Tests;

[Category(TestCategories.AdvancedFeatures)]
public sealed class AlertAndPopupTests : BrowserTestBase
{
    [Test]
    public async Task AlertAndPopup_HandleBoth_ShouldSucceed()
    {
        await Page.SetContentAsync(AlertAndPopupTestData.PageContent);

        string? alertMessage = null;
        Page.Dialog += async (_, dialog) =>
        {
            alertMessage = dialog.Message;
            await dialog.AcceptAsync();
        };

        await Page.Locator(AdvancedFeatureLocators.AlertButton).ClickAsync();
        Assert.That(alertMessage, Is.EqualTo(AlertAndPopupTestData.AlertMessage));

        var popup = await Page.RunAndWaitForPopupAsync(
            () => Page.Locator(AdvancedFeatureLocators.PopupButton).ClickAsync());
        await popup.SetContentAsync(AlertAndPopupTestData.PopupContent);
        await Expect(popup.Locator(AdvancedFeatureLocators.PopupHeading))
            .ToHaveTextAsync(AlertAndPopupTestData.PopupHeading);
        await popup.CloseAsync();
    }
}
