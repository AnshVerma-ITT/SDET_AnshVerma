using Allure.Net.Commons;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightAdvancedFeatures.Configuration;

namespace PlaywrightAdvancedFeatures.Fixtures;

public static class TestEvidence
{
    private const string FailureScreenshotName = "Failure screenshot";
    private const string TraceName = "Playwright trace";
    private const string VideoName = "Playwright video";
    private const string ImageContentType = "image/png";
    private const string TraceContentType = "application/zip";
    private const string VideoContentType = "video/webm";

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
        TestSettings settings,
        string evidenceDirectory)
    {
        var testName = GetSafeTestName();

        if (settings.ScreenshotOnFailure
            && TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            var screenshotPath = Path.Combine(evidenceDirectory, $"{testName}_Failure.png");
            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            AddAttachment(FailureScreenshotName, ImageContentType, screenshotPath);
        }

        if (settings.TraceEnabled)
        {
            var tracePath = Path.Combine(evidenceDirectory, $"{testName}_Trace.zip");
            await context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            AddAttachment(TraceName, TraceContentType, tracePath);
        }
    }

    public static async Task AttachVideo(IVideo video)
    {
        try
        {
            AddAttachment(VideoName, VideoContentType, await video.PathAsync());
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
