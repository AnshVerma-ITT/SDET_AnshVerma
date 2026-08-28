using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightAdvancedFeatures.Configuration;

namespace PlaywrightAdvancedFeatures.Fixtures;

[AllureNUnit]
public abstract class BrowserTestBase
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private TestSettings _settings = null!;

    protected IPage Page { get; private set; } = null!;
    protected string EvidenceDirectory { get; private set; } = null!;

    [SetUp]
    public async Task SetUp()
    {
        _settings = TestSettings.Load();
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await LaunchBrowser();
        EvidenceDirectory = GetProjectPath(_settings.EvidenceDirectory);
        Directory.CreateDirectory(EvidenceDirectory);

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            RecordVideoDir = _settings.VideoEnabled ? EvidenceDirectory : null
        });
        _context.SetDefaultTimeout(_settings.TimeoutMilliseconds);

        if (_settings.TraceEnabled)
        {
            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        Page = await _context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        var testName = GetSafeTestName();
        IVideo? video = _settings.VideoEnabled ? Page.Video : null;

        try
        {
            if (_settings.ScreenshotOnFailure &&
                TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var screenshot = Path.Combine(EvidenceDirectory, $"{testName}_Failure.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshot, FullPage = true });
                AddAttachment("Failure screenshot", "image/png", screenshot);
            }

            if (_settings.TraceEnabled)
            {
                var trace = Path.Combine(EvidenceDirectory, $"{testName}_Trace.zip");
                await _context.Tracing.StopAsync(new TracingStopOptions { Path = trace });
                AddAttachment("Playwright trace", "application/zip", trace);
            }
        }
        finally
        {
            await _context.CloseAsync();

            if (video is not null)
            {
                await AttachVideo(video);
            }

            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }

    protected static string GetProjectPath(params string[] parts)
    {
        var projectDirectory = Path.GetFullPath(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "../../.."));
        return Path.Combine([projectDirectory, .. parts]);
    }

    private Task<IBrowser> LaunchBrowser()
    {
        var browserName = Environment.GetEnvironmentVariable("BROWSER") ?? _settings.Browser;
        var options = new BrowserTypeLaunchOptions { Headless = _settings.Headless };

        return browserName.ToLowerInvariant() switch
        {
            "chromium" => _playwright.Chromium.LaunchAsync(options),
            "webkit" => _playwright.Webkit.LaunchAsync(options),
            _ => throw new ArgumentException("Browser must be Chromium or WebKit.")
        };
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
