using PlaywrightAdvancedFeatures.Locators;

namespace PlaywrightAdvancedFeatures.TestData;

public static class FileTransferTestData
{
    public const string AssetDirectory = "TestAssets";
    public const string UploadFileName = "upload-sample.txt";
    public const string DownloadFileName = "playwright-example.txt";
    public const string DownloadContent = "Playwright download example";
    public const string DownloadLinkText = "Download file";

    public static readonly string PageContent = $$"""
        <input id="{{AdvancedFeatureLocators.FileInputId}}" type="file">
        <a id="{{AdvancedFeatureLocators.DownloadLinkId}}"
           href="data:text/plain,{{DownloadContent}}"
           download="{{DownloadFileName}}">
          {{DownloadLinkText}}
        </a>
        """;
}
