using HRIntimeAutomation.Utilities;
using NUnit.Framework;

namespace HRIntimeAutomation.FrameworkTests;

[TestFixture]
[Category("framework")]
[Category("regression")]
public sealed class FailureArtifactTests
{
    [Test]
    [CancelAfter(60000)]
    public async Task FailedScenarioProducesScreenshotAndTrace()
    {
        var driver = new BrowserDriver();
        IReadOnlyList<string> artifactPaths = [];

        try
        {
            await driver.StartAsync();
            await driver.Page.SetContentAsync("<main><h1>Failure artifact probe</h1></main>");
            artifactPaths = await driver.StopAsync("Failure artifact probe");

            Assert.Multiple(() =>
            {
                Assert.That(artifactPaths.Any(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)),
                    Is.EqualTo(driver.Settings.ScreenshotOnFailure));
                Assert.That(artifactPaths.Any(path => path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)),
                    Is.EqualTo(driver.Settings.TraceOnFailure));
                Assert.That(artifactPaths.All(File.Exists), Is.True,
                    "One or more failure artifacts were not written to disk.");
            });
        }
        finally
        {
            if (artifactPaths.Count == 0)
                await driver.StopAsync();

            foreach (var artifactPath in artifactPaths.Where(File.Exists))
                File.Delete(artifactPath);
        }
    }
}
