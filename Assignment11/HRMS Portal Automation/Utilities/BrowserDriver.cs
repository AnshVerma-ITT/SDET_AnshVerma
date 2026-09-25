using System.Text.RegularExpressions;
using HRIntimeAutomation.Configuration;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Utilities;

public sealed class BrowserDriver
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private bool _tracingStarted;

    public IPage Page { get; private set; } = null!;
    public TestSettings Settings { get; private set; } = null!;

    public async Task StartAsync()
    {
        Settings = TestSettings.Load();
        try
        {
            _playwright = await Playwright.CreateAsync();
            _playwright.Selectors.SetTestIdAttribute(Settings.TestIdAttribute);
            _browser = await BrowserFactory.LaunchAsync(_playwright, Settings);
            _context = await BrowserContextFactory.CreateAsync(_browser, Settings);
            if (Settings.TraceOnFailure)
            {
                await _context.Tracing.StartAsync(new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
                _tracingStarted = true;
            }

            Page = await _context.NewPageAsync();
        }
        catch
        {
            await StopAsync();
            throw;
        }
    }

    public async Task<IReadOnlyList<string>> StopAsync(string? failedScenarioName = null)
    {
        var artifacts = new List<string>();
        try
        {
            if (_context is not null)
                await FinishArtifactsAsync(failedScenarioName, artifacts);

            if (_context is not null)
                await _context.CloseAsync();
        }
        finally
        {
            try
            {
                if (_browser is not null)
                    await _browser.CloseAsync();
            }
            finally
            {
                _playwright?.Dispose();
                _context = null;
                _browser = null;
                _playwright = null;
                _tracingStarted = false;
            }
        }

        return artifacts;
    }

    private async Task FinishArtifactsAsync(string? failedScenarioName, ICollection<string> artifacts)
    {
        var failed = !string.IsNullOrWhiteSpace(failedScenarioName);
        string? artifactBasePath = null;
        if (failed)
        {
            var artifactsDirectory = Settings.GetArtifactsDirectory();
            Directory.CreateDirectory(artifactsDirectory);
            var safeScenarioName = Regex.Replace(failedScenarioName!, @"[^A-Za-z0-9._-]+", "_").Trim('_');
            var uniqueSuffix = $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}_{Guid.NewGuid():N}";
            artifactBasePath = Path.Combine(artifactsDirectory, $"{safeScenarioName}_{uniqueSuffix}");
        }

        if (failed && Settings.ScreenshotOnFailure && !Page.IsClosed)
        {
            var screenshotPath = $"{artifactBasePath}.png";
            await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
            artifacts.Add(screenshotPath);
        }

        if (!_tracingStarted || _context is null)
            return;

        if (failed && Settings.TraceOnFailure)
        {
            var tracePath = $"{artifactBasePath}.zip";
            await _context.Tracing.StopAsync(new() { Path = tracePath });
            artifacts.Add(tracePath);
        }
        else
        {
            await _context.Tracing.StopAsync();
        }

        _tracingStarted = false;
    }
}
