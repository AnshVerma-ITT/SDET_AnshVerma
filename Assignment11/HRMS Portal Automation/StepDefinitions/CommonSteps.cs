using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class CommonSteps
{
    private readonly ScenarioTestContext _context;

    public CommonSteps(ScenarioTestContext context) => _context = context;

    [Given("I am logged in with valid credentials")]
    public async Task GivenIAmLoggedInWithValidCredentials()
    {
        var credentials = CredentialProvider.GetRequiredAdminCredentials();
        await _context.LoginPage.OpenAsync();
        await _context.LoginPage.LoginAsync(credentials.Username, credentials.Password);
        await _context.LoginPage.WaitForDashboardAsync();
    }
}
