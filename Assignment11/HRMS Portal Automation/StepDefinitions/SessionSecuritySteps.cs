using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.Models;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class SessionSecuritySteps
{
    private readonly ScenarioTestContext _context;

    public SessionSecuritySteps(ScenarioTestContext context) => _context = context;

    [When("I attempt direct Dashboard access refresh and back navigation")]
    public async Task WhenIAttemptDirectDashboardAccessRefreshAndBackNavigation()
    {
        var page = _context.Driver.Page;

        await page.GotoAsync(AppRoutes.Dashboard, new() { WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded });
        var directAccessShowsLogin = await _context.LoginPage.IsDisplayedAsync();
        var directAccessUrl = page.Url;

        await page.ReloadAsync(new() { WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded });
        var refreshShowsLogin = await _context.LoginPage.IsDisplayedAsync();
        var refreshUrl = page.Url;

        await page.GoBackAsync(new() { WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded });
        var backNavigationShowsLogin = await _context.LoginPage.IsDisplayedAsync();

        _context.SessionSecurityResult = new SessionSecurityResult(
            directAccessShowsLogin,
            directAccessUrl,
            refreshShowsLogin,
            refreshUrl,
            backNavigationShowsLogin,
            page.Url);
    }

    [Then("every post-logout attempt should still require authentication")]
    public void ThenEveryPostLogoutAttemptShouldStillRequireAuthentication()
    {
        Assert.That(_context.SessionSecurityResult, Is.Not.Null,
            "Post-logout session checks were not captured.");
        var result = _context.SessionSecurityResult!;

        Assert.Multiple(() =>
        {
            Assert.That(result.DirectAccessShowsLogin, Is.True,
                "Direct Dashboard access succeeded after logout.");
            Assert.That(result.RefreshShowsLogin, Is.True,
                "Refreshing after logout restored an authenticated page.");
            Assert.That(result.BackNavigationShowsLogin, Is.True,
                "Browser back navigation restored an authenticated page.");
            Assert.That(GetPath(result.DirectAccessUrl), Is.Not.EqualTo(AppRoutes.Dashboard).IgnoreCase,
                "Direct access must not remain on the protected Dashboard path.");
            Assert.That(GetPath(result.RefreshUrl), Is.Not.EqualTo(AppRoutes.Dashboard).IgnoreCase,
                "Refresh must not restore the protected Dashboard path.");
            Assert.That(GetPath(result.BackNavigationUrl), Is.Not.EqualTo(AppRoutes.Dashboard).IgnoreCase,
                "Back navigation must not restore the protected Dashboard path.");
        });
    }

    private static string GetPath(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.AbsolutePath : url;
}
