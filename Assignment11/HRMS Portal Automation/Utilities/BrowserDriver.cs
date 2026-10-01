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

    private IPage? _page;
    private TestSettings? _settings;

    public IPage Page => _page ?? throw new InvalidOperationException("BrowserDriver has not been started.");
    public TestSettings Settings => _settings ?? throw new InvalidOperationException("BrowserDriver has not been started.");

    public async Task StartAsync()
    {
        _settings = TestSettings.Load();
        try
        {
            _playwright = await Playwright.CreateAsync();
            _playwright.Selectors.SetTestIdAttribute(Settings.TestIdAttribute);
            _browser = await BrowserFactory.LaunchAsync(_playwright, Settings);
            _context = await BrowserContextFactory.CreateAsync(_browser, Settings);
            if (Settings.TraceOnFailure && Settings.FailureArtifactsEnabled)
            {
                await _context.Tracing.StartAsync(new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
                _tracingStarted = true;
            }

            _page = await _context.NewPageAsync();
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
                await CaptureFailureArtifactsAndStopTracingAsync(failedScenarioName, artifacts);

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
                _page = null;
                _tracingStarted = false;
            }
        }

        return artifacts;
    }

    private async Task CaptureFailureArtifactsAndStopTracingAsync(
        string? failedScenarioName,
        ICollection<string> artifacts)
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

        if (failed && Settings.FailureArtifactsEnabled && Settings.ScreenshotOnFailure && !Page.IsClosed)
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
