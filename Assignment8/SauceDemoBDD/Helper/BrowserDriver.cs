using Allure.Net.Commons;
using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemoBDD.Configuration;
using SauceDemoBDD.Pages;

namespace SauceDemoBDD.Support;

public sealed class BrowserDriver
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    public TestSettings Settings { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;
    public LoginPage LoginPage { get; private set; } = null!;
    public InventoryPage InventoryPage { get; private set; } = null!;
    public CartPage CartPage { get; private set; } = null!;
    public CheckoutPage CheckoutPage { get; private set; } = null!;

    public async Task Start()
    {
        try
        {
            Settings = TestSettings.Load();
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _playwright.Selectors.SetTestIdAttribute(Settings.TestIdAttribute);
            _browser = await LaunchBrowser(_playwright);

            var evidenceDirectory = GetEvidenceDirectory();
            Directory.CreateDirectory(evidenceDirectory);

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                BaseURL = Settings.BaseUrl,
                ViewportSize = new ViewportSize
                {
                    Width = Settings.ViewportWidth,
                    Height = Settings.ViewportHeight
                },
                RecordVideoDir = Settings.VideoEnabled ? evidenceDirectory : null
            });
            _context.SetDefaultTimeout(Settings.TimeoutMilliseconds);
            _context.SetDefaultNavigationTimeout(Settings.TimeoutMilliseconds);

            if (Settings.TraceEnabled)
            {
                await _context.Tracing.StartAsync(new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
            }

            Page = await _context.NewPageAsync();
            LoginPage = new LoginPage(Page);
            InventoryPage = new InventoryPage(Page);
            CartPage = new CartPage(Page);
            CheckoutPage = new CheckoutPage(Page);
        }
        catch
        {
            await CloseResources();
            throw;
        }
    }

    public async Task Stop(bool scenarioFailed)
    {
        if (_context is null)
        {
            await CloseResources();
            return;
        }

        var evidenceDirectory = GetEvidenceDirectory();
        var testName = GetSafeTestName();
        IVideo? video = Settings.VideoEnabled ? Page.Video : null;

        if (scenarioFailed && Settings.ScreenshotOnFailure)
        {
            await TryRun("capture the failure screenshot", async () =>
            {
                var screenshot = Path.Combine(evidenceDirectory, $"{testName}_Failure.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshot, FullPage = true });
                AddAttachment("Failure screenshot", "image/png", screenshot);
            });
        }

        if (Settings.TraceEnabled)
        {
            await TryRun("save the Playwright trace", async () =>
            {
                var trace = Path.Combine(evidenceDirectory, $"{testName}_Trace.zip");
                await _context.Tracing.StopAsync(new TracingStopOptions { Path = trace });
                AddAttachment("Playwright trace", "application/zip", trace);
            });
        }

        await CloseResources(video);
    }

    private Task<IBrowser> LaunchBrowser(IPlaywright playwright)
    {
        var browserName = Environment.GetEnvironmentVariable("BROWSER") ?? Settings.Browser;
        var options = new BrowserTypeLaunchOptions { Headless = Settings.Headless };

        return browserName.ToLowerInvariant() switch
        {
            "chromium" => playwright.Chromium.LaunchAsync(options),
            "webkit" => playwright.Webkit.LaunchAsync(options),
            _ => throw new ArgumentException("Browser must be Chromium or WebKit.")
        };
    }

    private async Task CloseResources(IVideo? video = null)
    {
        if (_context is not null)
        {
            var context = _context;
            _context = null;
            await TryRun("close the browser context", () => context.CloseAsync());
        }

        if (video is not null)
        {
            await TryRun("attach the Playwright video", () => AttachVideo(video));
        }

        if (_browser is not null)
        {
            var browser = _browser;
            _browser = null;
            await TryRun("close the browser", () => browser.CloseAsync());
        }

        _playwright?.Dispose();
        _playwright = null;
    }

    private string GetEvidenceDirectory()
    {
        return Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            Settings.EvidenceDirectory);
    }

    private static async Task AttachVideo(IVideo video)
    {
        try
        {
            AddAttachment("Playwright video", "video/webm", await video.PathAsync());
        }
        catch (PlaywrightException exception) when (
            exception.Message.Contains("did not produce any video frames", StringComparison.OrdinalIgnoreCase))
        {
            TestContext.Progress.WriteLine("The page produced no video frames.");
        }
    }

    private static async Task TryRun(string operation, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            TestContext.Progress.WriteLine($"Could not {operation}: {exception.Message}");
        }
    }

    private static void AddAttachment(string name, string contentType, string path)
    {
        TestContext.AddTestAttachment(path, name);
        try
        {
            AllureApi.AddAttachment(name, contentType, path);
        }
        catch
        {
            // NUnit keeps the attachment if Allure is unavailable.
        }
    }

    private static string GetSafeTestName()
    {
        var invalid = Path.GetInvalidFileNameChars().ToHashSet();
        return new string(TestContext.CurrentContext.Test.Name
            .Select(character => invalid.Contains(character) ? '_' : character)
            .ToArray());
    }
}
