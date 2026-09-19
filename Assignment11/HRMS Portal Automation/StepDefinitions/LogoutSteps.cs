using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class LogoutSteps
{
    private readonly ScenarioTestContext _context;

    public LogoutSteps(ScenarioTestContext context) => _context = context;

    [When("I logout from the profile menu")]
    public Task WhenILogoutFromTheProfileMenu() =>
        _context.LogoutPage.LogoutAsync(ProfileTestData.AvatarInitials);

    [Then("I should be redirected to the login page")]
    public async Task ThenIShouldBeRedirectedToTheLoginPage() =>
        Assert.That(await _context.LoginPage.IsDisplayedAsync(), Is.True,
            "The Login page was not displayed after logout.");
}
