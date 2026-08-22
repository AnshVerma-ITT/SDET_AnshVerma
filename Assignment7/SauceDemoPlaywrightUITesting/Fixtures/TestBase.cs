using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;

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
        Browser = await LaunchBrowserAsync();

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = Settings.BaseUrl,
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
        });

        Context.SetDefaultTimeout(Settings.TimeoutMilliseconds);
        Context.SetDefaultNavigationTimeout(Settings.TimeoutMilliseconds);

        if (Settings.TraceEnabled)
        {
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var projectDirectory = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../.."));
        var evidenceDirectory = Path.Combine(projectDirectory, "TestResults");
        Directory.CreateDirectory(evidenceDirectory);

        if (failed && Settings.ScreenshotOnFailure && Page is not null)
        {
            var screenshotPath = Path.Combine(evidenceDirectory, $"{SafeTestName()}_Failure.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true,
                Timeout = 10000
            });

            TestContext.AddTestAttachment(screenshotPath, "Failure screenshot");
            TryAddAllureAttachment("Failure screenshot", "image/png", screenshotPath);
        }

        if (Settings.TraceEnabled && Context is not null)
        {
            var tracePath = Path.Combine(evidenceDirectory, $"{SafeTestName()}_Trace.zip");
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });

            TestContext.AddTestAttachment(tracePath, "Playwright trace");
            TryAddAllureAttachment("Playwright trace", "application/zip", tracePath);
        }

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

    protected Task StepAsync(string name, Func<Task> action)
    {
        return AllureApi.Step(name, action);
    }

    protected async Task<InventoryPage> OpenAndLoginAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync();
        await loginPage.LoginAsync(LoginTestData.ValidUsername, LoginTestData.ValidPassword);
        await Page.WaitForURLAsync("**/inventory.html");

        return new InventoryPage(Page);
    }

    private async Task<IBrowser> LaunchBrowserAsync()
    {
        return BrowserEngine switch
        {
            BrowserEngine.Chrome => await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = Settings.Headless,
                Channel = Settings.UseChromeChannel ? "chrome" : null
            }),
            BrowserEngine.WebKit => await Playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = Settings.Headless
            }),
            _ => throw new NotSupportedException($"Unsupported browser engine: {BrowserEngine}")
        };
    }

    private static void TryAddAllureAttachment(string name, string type, string path)
    {
        try
        {
            AllureApi.AddAttachment(name, type, path);
        }
        catch
        {
            // NUnit attachments are still available if Allure is not active in the current runner.
        }
    }

    private string SafeTestName()
    {
        var raw = $"{BrowserEngine}_{TestContext.CurrentContext.Test.Name}";
        var invalid = Path.GetInvalidFileNameChars();
        return new string(raw.Select(character => invalid.Contains(character) ? '_' : character).ToArray());
    }
}
