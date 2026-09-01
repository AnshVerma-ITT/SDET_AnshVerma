using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;

namespace SauceDemo.Playwright.Tests.Fixtures;

[AllureNUnit]
public abstract class TestBase
{
    protected TestBase(BrowserEngine browserEngine)
    {
        BrowserEngine = browserEngine;
    }

    protected BrowserEngine BrowserEngine { get; }
    protected TestSettings Settings { get; private set; } = null!;
    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        Settings = TestSettings.Load();
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await BrowserFactory.Launch(Playwright, BrowserEngine, Settings);

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = Settings.BaseUrl,
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        });

        Context.SetDefaultTimeout(Settings.TimeoutMilliseconds);
        Context.SetDefaultNavigationTimeout(Settings.TimeoutMilliseconds);

        await TestEvidence.StartTracing(Context, Settings);
        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        try
        {
            if (Page is not null && Context is not null)
            {
                await TestEvidence.Capture(Page, Context, BrowserEngine, Settings);
            }
        }
        finally
        {
            if (Context is not null)
            {
                await Context.CloseAsync();
            }

            if (Browser is not null)
            {
                await Browser.CloseAsync();
            }

            Playwright?.Dispose();
        }
    }
}
