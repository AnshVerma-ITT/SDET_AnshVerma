using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SauceDemo.Playwright.Tests.Configuration;

namespace SauceDemo.Playwright.Tests.Fixtures;

public static class TestEvidence
{
    public static Task StartTracing(IBrowserContext context, TestSettings settings)
    {
        if (!settings.TraceEnabled)
        {
            return Task.CompletedTask;
        }

        return context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    public static async Task Capture(
        IPage page,
        IBrowserContext context,
        BrowserEngine browserEngine,
        TestSettings settings,
        bool? testFailed = null)
    {
        var evidenceDirectory = GetEvidenceDirectory();
        Directory.CreateDirectory(evidenceDirectory);
        var safeTestName = GetSafeTestName(browserEngine);
        var failed = testFailed ?? TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;

        if (failed && settings.ScreenshotOnFailure)
        {
            var screenshotPath = Path.Combine(evidenceDirectory, $"{safeTestName}_Failure.png");
            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true,
                Timeout = 10000
            });

            TestContext.AddTestAttachment(screenshotPath, "Failure screenshot");
            AllureHelper.AddAttachment("Failure screenshot", "image/png", screenshotPath);
        }

        if (settings.TraceEnabled)
        {
            var tracePath = Path.Combine(evidenceDirectory, $"{safeTestName}_Trace.zip");
            await context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });

            TestContext.AddTestAttachment(tracePath, "Playwright trace");
            AllureHelper.AddAttachment("Playwright trace", "application/zip", tracePath);
        }
    }

    public static async Task AttachVideo(IVideo video)
    {
        try
        {
            var videoPath = await video.PathAsync();
            TestContext.AddTestAttachment(videoPath, "Playwright video");
            AllureHelper.AddAttachment("Playwright video", "video/webm", videoPath);
        }
        catch (PlaywrightException exception) when (
            exception.Message.Contains("did not produce any video frames", StringComparison.OrdinalIgnoreCase))
        {
            TestContext.Progress.WriteLine("No video was attached because the page produced no video frames.");
        }
    }

    public static string GetEvidenceDirectory()
    {
        var projectDirectory = Path.GetFullPath(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "../../.."));
        return Path.Combine(projectDirectory, "TestResults");
    }

    private static string GetSafeTestName(BrowserEngine browserEngine)
    {
        var raw = $"{browserEngine}_{TestContext.CurrentContext.Test.Name}";
        const string windowsInvalidCharacters = "<>:\"/\\|?*";
        var invalid = Path.GetInvalidFileNameChars().Concat(windowsInvalidCharacters).ToHashSet();
        return new string(raw.Select(character => invalid.Contains(character) ? '_' : character).ToArray());
    }
}
