using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightAdvancedFeatures.Configuration;
using PlaywrightAdvancedFeatures.Infrastructure;

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
        _browser = await BrowserFactory.Launch(_playwright, _settings);
        EvidenceDirectory = TestPaths.InTestOutput(_settings.EvidenceDirectory);
        Directory.CreateDirectory(EvidenceDirectory);

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = _settings.ViewportWidth,
                Height = _settings.ViewportHeight
            },
            RecordVideoDir = _settings.VideoEnabled ? EvidenceDirectory : null
        });
        _context.SetDefaultTimeout(_settings.TimeoutMilliseconds);
        await TestEvidence.StartTracing(_context, _settings);
        Page = await _context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        IVideo? video = _settings.VideoEnabled ? Page.Video : null;

        try
        {
            await TestEvidence.Capture(Page, _context, _settings, EvidenceDirectory);
        }
        finally
        {
            await _context.CloseAsync();

            if (video is not null)
            {
                await TestEvidence.AttachVideo(video);
            }

            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}
