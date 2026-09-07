using PlaywrightAdvancedFeatures.Locators;

namespace PlaywrightAdvancedFeatures.TestData;

public static class AlertAndPopupTestData
{
    public const string AlertMessage = "Assignment alert";
    public const string AlertButtonText = "Show alert";
    public const string PopupButtonText = "Open popup";
    public const string PopupHeading = "Popup opened";

    public static readonly string PageContent = $$"""
        <button id="{{AdvancedFeatureLocators.AlertButtonId}}"
                onclick="alert('{{AlertMessage}}')">{{AlertButtonText}}</button>
        <button id="{{AdvancedFeatureLocators.PopupButtonId}}"
                onclick="window.open('about:blank', '_blank')">{{PopupButtonText}}</button>
        """;

    public static readonly string PopupContent =
        $"<h1 id=\"{AdvancedFeatureLocators.PopupHeadingId}\">{PopupHeading}</h1>";
}
