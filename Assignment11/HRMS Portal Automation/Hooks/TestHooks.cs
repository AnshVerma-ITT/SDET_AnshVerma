using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.Pages;
using HRIntimeAutomation.Utilities;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.Hooks;

[Binding]
public sealed class TestHooks
{
    private static readonly SemaphoreSlim MutationGate = new(1, 1);
    private readonly ScenarioTestContext _context;
    private readonly ScenarioContext _scenarioContext;
    private bool _mutationGateHeld;

    public TestHooks(ScenarioTestContext context, ScenarioContext scenarioContext)
    {
        _context = context;
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario(Order = 0)]
    public void EnforceSafetyPolicy()
    {
        var settings = TestSettings.Load();
        if (HasTag(TestTags.AccountRisk) && !settings.AllowAccountRiskTests)
            Assert.Ignore("Account-risk login variations are disabled. Set ALLOW_ACCOUNT_RISK_TESTS=true to opt in.");

        if (HasTag(TestTags.DataMutation) && settings.IsProduction && !settings.AllowProductionMutations)
            Assert.Ignore("Production data mutations are disabled. Set ALLOW_PRODUCTION_MUTATIONS=true only for an approved run.");
    }

    [BeforeScenario(Order = 10)]
    public async Task SerializeMutatingScenariosAsync()
    {
        if (!HasTag(TestTags.DataMutation))
            return;
        await MutationGate.WaitAsync();
        _mutationGateHeld = true;
    }

    [BeforeScenario(Order = 20)]
    public async Task StartBrowserAsync()
    {
        _context.Driver = new BrowserDriver();
        await _context.Driver.StartAsync();
        var page = _context.Driver.Page;
        var baseUrl = _context.Driver.Settings.BaseUrl;
        _context.LoginPage = new(page, baseUrl);
        _context.NavigationPage = new(page, baseUrl);
        _context.DashboardPage = new(page, baseUrl);
        _context.MyProfilePage = new(page, baseUrl);
        _context.LeaveApplicationPage = new(page, baseUrl);
        _context.AttendanceRecordPage = new(page, baseUrl);
        _context.LeaveCorrectionPage = new(page, baseUrl);
        _context.EmployeeDirectoryPage = new(page, baseUrl);
        _context.FooterPage = new(page, baseUrl);
        _context.LogoutPage = new(page, baseUrl);
    }

    [AfterScenario(Order = 100)]
    public async Task StopBrowserAsync()
    {
        try
        {
            if (_context.Driver is null)
                return;
            var failedScenarioName = _scenarioContext.TestError is null ? null : _scenarioContext.ScenarioInfo.Title;
            var artifacts = await _context.Driver.StopAsync(failedScenarioName);
            foreach (var artifact in artifacts.Where(File.Exists))
                TestContext.AddTestAttachment(artifact);
        }
        finally
        {
            if (_mutationGateHeld)
            {
                MutationGate.Release();
                _mutationGateHeld = false;
            }
        }
    }

    private bool HasTag(string tag) => _scenarioContext.ScenarioInfo.CombinedTags
        .Any(value => value.Equals(tag, StringComparison.OrdinalIgnoreCase));
}
