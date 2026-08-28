using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightAdvancedFeatures.Fixtures;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightAdvancedFeatures.Tests;

[Category("AdvancedFeatures")]
public sealed class AdvancedBrowserFeaturesTests : BrowserTestBase
{
    [Test]
    public async Task FileTransfer_UploadAndDownload_ShouldSucceed()
    {
        await Page.SetContentAsync(
            """
            <input id="file-input" type="file">
            <a href="data:text/plain,Playwright download example" download="playwright-example.txt">
              Download file
            </a>
            """);

        var uploadPath = GetProjectPath("TestAssets", "upload-sample.txt");
        var fileInput = Page.Locator("#file-input");
        await fileInput.SetInputFilesAsync(uploadPath);
        Assert.That(await fileInput.InputValueAsync(), Does.EndWith("upload-sample.txt"));

        var download = await Page.RunAndWaitForDownloadAsync(
            () => Page.GetByRole(AriaRole.Link, new() { Name = "Download file" }).ClickAsync());
        var downloadPath = Path.Combine(EvidenceDirectory, download.SuggestedFilename);
        await download.SaveAsAsync(downloadPath);

        Assert.That(download.SuggestedFilename, Is.EqualTo("playwright-example.txt"));
        Assert.That(await File.ReadAllTextAsync(downloadPath), Does.Contain("Playwright download example"));
    }

    [Test]
    public async Task BrowserWindows_HandleDialogAndPopup_ShouldSucceed()
    {
        await Page.SetContentAsync(
            """
            <button id="alert-button" onclick="alert('Assignment alert')">Show alert</button>
            <button id="popup-button" onclick="window.open('about:blank', '_blank')">Open popup</button>
            """);

        string? alertMessage = null;
        Page.Dialog += async (_, dialog) =>
        {
            alertMessage = dialog.Message;
            await dialog.AcceptAsync();
        };

        await Page.Locator("#alert-button").ClickAsync();
        Assert.That(alertMessage, Is.EqualTo("Assignment alert"));

        var popup = await Page.RunAndWaitForPopupAsync(
            () => Page.Locator("#popup-button").ClickAsync());
        await popup.SetContentAsync("<h1>Popup opened</h1>");
        await Expect(popup.GetByRole(AriaRole.Heading)).ToHaveTextAsync("Popup opened");
        await popup.CloseAsync();
    }

    [Test]
    public async Task IFrame_InteractUsingFrameLocator_ShouldSucceed()
    {
        await Page.SetContentAsync(
            """
            <iframe title="lesson-frame"
              srcdoc="<button onclick=&quot;document.querySelector('p').textContent='Frame handled'&quot;>Update frame</button><p>Waiting</p>">
            </iframe>
            """);

        var frame = Page.FrameLocator("iframe[title='lesson-frame']");
        await frame.GetByRole(AriaRole.Button, new() { Name = "Update frame" }).ClickAsync();
        await Expect(frame.Locator("p")).ToHaveTextAsync("Frame handled");
    }

    [TestCase(2, 3, 5, 10)]
    [TestCase(10, 20, 30, 60)]
    public async Task JavaScript_EvaluateParameterizedValues_ShouldReturnTotal(
        int first,
        int second,
        int third,
        int expectedTotal)
    {
        var actualTotal = await Page.EvaluateAsync<int>(
            "values => values.reduce((total, value) => total + value, 0)",
            new[] { first, second, third });

        Assert.That(actualTotal, Is.EqualTo(expectedTotal));
    }

    [Test]
    public async Task Network_InterceptApiCall_ShouldReturnMockedProduct()
    {
        var requestWasIntercepted = false;
        await Page.RouteAsync("**/api/products", async route =>
        {
            requestWasIntercepted = true;
            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 200,
                ContentType = "application/json",
                Headers = new Dictionary<string, string> { ["Access-Control-Allow-Origin"] = "*" },
                Body = """{"name":"Mock Sauce Labs Backpack"}"""
            });
        });

        await Page.SetContentAsync(
            """
            <button id="load-product">Load product</button>
            <p id="product-name"></p>
            <script>
              document.querySelector('#load-product').addEventListener('click', async () => {
                const response = await fetch('https://training.test/api/products');
                const product = await response.json();
                document.querySelector('#product-name').textContent = product.name;
              });
            </script>
            """);

        await Page.Locator("#load-product").ClickAsync();
        await Expect(Page.Locator("#product-name")).ToHaveTextAsync("Mock Sauce Labs Backpack");
        Assert.That(requestWasIntercepted, Is.True);
    }
}
