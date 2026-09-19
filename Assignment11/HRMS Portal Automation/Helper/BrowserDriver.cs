using HRIntimeAutomation.Configuration;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Helper;

public sealed class BrowserDriver
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IDisposable? _executionSlot;

    public IPage Page { get; private set; } = null!;
    public TestSettings Settings { get; private set; } = null!;

    public async Task StartAsync()
    {
        Settings = TestSettings.Load();
        _executionSlot = await BrowserExecutionLimiter.AcquireAsync(Settings);

        try
        {
            _playwright = await Playwright.CreateAsync();
            _playwright.Selectors.SetTestIdAttribute(Settings.TestIdAttribute);
            _browser = await BrowserFactory.LaunchAsync(_playwright, Settings);
            _context = await BrowserContextFactory.CreateAsync(_browser, Settings);
            Page = await _context.NewPageAsync();
        }
        catch
        {
            await StopAsync();
            throw;
        }
    }

    public async Task StopAsync()
    {
        try
        {
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
                Interlocked.Exchange(ref _executionSlot, null)?.Dispose();
            }
        }
    }
}
