using PlaywrightApiFramework.Framework.Fixtures;
using PlaywrightApiFramework.ReqRes.Configuration;
using PlaywrightApiFramework.ReqRes.Services;
using PlaywrightApiFramework.Tests.DataProviders;
using NUnit.Framework;

namespace PlaywrightApiFramework.Tests.Base;

public class ApiTestBase
{
    public ApiFixture Fixture { get; set; }
    public UserService UserService { get; set; }
    public ReqResSettings ReqResConfig { get; set; }
    public UserScenarioData TestData { get; set; }

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
