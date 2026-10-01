using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class SessionSecuritySteps
{
    private readonly ScenarioTestContext _context;
    public SessionSecuritySteps(ScenarioTestContext context) => _context = context;

    [When("I attempt direct access to every protected route then refresh and navigate back")]
    public async Task CheckProtectedRoutes()
    {
        var results = new Dictionary<string, bool>();
        foreach (var route in AppRoutes.ProtectedRoutes)
        {
            await _context.Driver!.Page.GotoAsync(route, new() { WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded });
            results[$"direct:{route}"] = await IsLoginAsync();
        }
        await _context.Driver!.Page.ReloadAsync(new() { WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded });
        results["refresh"] = await IsLoginAsync();
        await _context.Driver.Page.GoBackAsync(new() { WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded });
        results["back"] = await IsLoginAsync();
        _context.SessionSecurityChecks = results;
    }

    [Then("every post-logout attempt should still require authentication")]
    public void EveryAttemptRequiresLogin()
    {
        Assert.That(_context.SessionSecurityChecks, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var result in _context.SessionSecurityChecks)
                Assert.That(result.Value, Is.True, $"Post-logout check failed: {result.Key}");
        });
    }

    private async Task<bool> IsLoginAsync() =>
        await _context.LoginPage.IsDisplayedAsync()
        && _context.LoginPage.Pages.RouteMatches(_context.Driver!.Page.Url, AppRoutes.Login);
}
