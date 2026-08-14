using Microsoft.Playwright;
using PlaywrightApiFramework.Framework.API;
using PlaywrightApiFramework.Framework.Authentication;
using PlaywrightApiFramework.Framework.Configuration;

namespace PlaywrightApiFramework.Framework.Fixtures;

public class ApiFixture
{
    public AppSettings Settings { get; set; }
    public IPlaywright Playwright { get; set; }
    public IAPIRequestContext Request { get; set; }
    public ApiClient Client { get; set; }

    public async Task StartAsync()
    {
        Settings = AppSettings.Load();
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = Settings.BaseUrl,
            ExtraHTTPHeaders = AuthManager.GetHeaders(Settings.ApiKey),
            Timeout = PlaywrightConfig.Timeout
        });
        Client = new ApiClient(Request);
    }

    public async Task StopAsync()
    {
        if (Request != null)
        {
            await Request.DisposeAsync();
        }
        if (Playwright != null)
        {
            Playwright.Dispose();
        }
    }
}
