using PlaywrightApiFramework.Framework.Fixtures;
using PlaywrightApiFramework.ReqRes.Configuration;
using PlaywrightApiFramework.ReqRes.Services;
using PlaywrightApiFramework.Tests.DataProviders;
using NUnit.Framework;

namespace PlaywrightApiFramework.Tests.Base;

public class ApiTestBase
{
    protected ApiFixture Fixture { get; private set; }
    protected UserService UserService { get; private set; }
    protected ReqResSettings ReqResConfig { get; private set; }
    protected UserScenarioData TestData { get; private set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Fixture = new ApiFixture();
        await Fixture.StartAsync();
        UserService = new UserService(Fixture.Client);
        ReqResConfig = ReqResSettings.Load();
        TestData = UserDataProvider.ScenarioData();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Fixture.StopAsync();
    }
}
