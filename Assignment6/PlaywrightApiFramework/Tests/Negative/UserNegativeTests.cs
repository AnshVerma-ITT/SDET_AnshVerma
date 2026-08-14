using Microsoft.Playwright;
using PlaywrightApiFramework.Framework.Fixtures;
using PlaywrightApiFramework.Framework.Reporting;
using PlaywrightApiFramework.Framework.Utilities;
using PlaywrightApiFramework.ReqRes.Endpoints;
using PlaywrightApiFramework.ReqRes.Services;
using NUnit.Framework;

namespace PlaywrightApiFramework.Tests.Negative;

[TestFixture]
public class UserNegativeTests
{
    public ApiFixture Fixture { get; set; }
    public UserService UserService { get; set; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Fixture = new ApiFixture();
        await Fixture.StartAsync();
        UserService = new UserService(Fixture.Client);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Fixture.StopAsync();
    }

    [Test]
    public async Task GetMissingUser_ShouldReturnNotFound()
    {
        ReportHelper.PrintTest("Negative - missing user");
        var response = await UserService.GetUser(23);
        var body = await response.TextAsync();
        ReportHelper.PrintResponse("GET /api/users/23", response);
        ReportHelper.PrintValue("Response body", body.Trim());
        Assert.That(response.Status, Is.EqualTo(404));
        Assert.That(body.Trim(), Is.EqualTo("{}"));
    }

    [Test]
    public async Task RegisterWithoutPassword_ShouldReturnBadRequest()
    {
        ReportHelper.PrintTest("Negative - register without password");
        var response = await UserService.Register(new
        {
            email = "sydney@fife"
        });
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("POST /api/register", response);
        ReportHelper.PrintValue("Error", JsonHelper.GetString(json, "error"));
        Assert.That(response.Status, Is.EqualTo(400));
        Assert.That(JsonHelper.GetString(json, "error"), Is.Not.Empty);
    }

    [Test]
    public async Task MissingApiKey_ShouldReturnUnauthorized()
    {
        ReportHelper.PrintTest("Negative - missing API key");
        using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        var request = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = Fixture.Settings.BaseUrl
        });
        var response = await request.GetAsync(UserEndpoints.SingleUser(2));
        var json = await JsonHelper.GetJson(response);
        ReportHelper.PrintResponse("GET /api/users/2 without x-api-key", response);
        ReportHelper.PrintValue("Error", JsonHelper.GetString(json, "error"));
        Assert.That(response.Status, Is.EqualTo(401));
        Assert.That(JsonHelper.GetString(json, "error"), Is.Not.Empty);
        await request.DisposeAsync();
    }
}
