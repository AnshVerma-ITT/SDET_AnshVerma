using PlaywrightAdvancedFeatures.Locators;

namespace PlaywrightAdvancedFeatures.TestData;

public static class IFrameTestData
{
    public const string FrameTitle = "lesson-frame";
    public const string ButtonText = "Update frame";
    public const string InitialMessage = "Waiting";
    public const string UpdatedMessage = "Frame handled";

    public static readonly string PageContent = $$"""
        <iframe id="{{AdvancedFeatureLocators.FrameId}}" title="{{FrameTitle}}"
          srcdoc="<button id=&quot;{{AdvancedFeatureLocators.FrameButtonId}}&quot; onclick=&quot;document.querySelector('#{{AdvancedFeatureLocators.FrameMessageId}}').textContent='{{UpdatedMessage}}'&quot;>{{ButtonText}}</button><p id=&quot;{{AdvancedFeatureLocators.FrameMessageId}}&quot;>{{InitialMessage}}</p>">
        </iframe>
        """;
}
