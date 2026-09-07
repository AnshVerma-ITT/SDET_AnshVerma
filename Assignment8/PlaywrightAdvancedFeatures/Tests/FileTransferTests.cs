using NUnit.Framework;
using PlaywrightAdvancedFeatures.Configuration;
using PlaywrightAdvancedFeatures.Fixtures;
using PlaywrightAdvancedFeatures.Infrastructure;
using PlaywrightAdvancedFeatures.Locators;
using PlaywrightAdvancedFeatures.TestData;

namespace PlaywrightAdvancedFeatures.Tests;

[Category(TestCategories.AdvancedFeatures)]
public sealed class FileTransferTests : BrowserTestBase
{
    [Test]
    public async Task FileTransfer_UploadAndDownload_ShouldSucceed()
    {
        await Page.SetContentAsync(FileTransferTestData.PageContent);

        var uploadPath = TestPaths.InTestOutput(
            FileTransferTestData.AssetDirectory,
            FileTransferTestData.UploadFileName);
        var fileInput = Page.Locator(AdvancedFeatureLocators.FileInput);
        await fileInput.SetInputFilesAsync(uploadPath);
        Assert.That(
            await fileInput.InputValueAsync(),
            Does.EndWith(FileTransferTestData.UploadFileName));

        var download = await Page.RunAndWaitForDownloadAsync(
            () => Page.Locator(AdvancedFeatureLocators.DownloadLink).ClickAsync());
        var downloadPath = Path.Combine(EvidenceDirectory, download.SuggestedFilename);
        await download.SaveAsAsync(downloadPath);

        Assert.That(download.SuggestedFilename, Is.EqualTo(FileTransferTestData.DownloadFileName));
        Assert.That(
            await File.ReadAllTextAsync(downloadPath),
            Does.Contain(FileTransferTestData.DownloadContent));
    }
}
