using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.Utilities;
using HRIntimeAutomation.Pages;
using Reqnroll;

namespace HRIntimeAutomation.Hooks;

[Binding]
public sealed class TestHooks
{
    private readonly ScenarioTestContext _context;
    private readonly ScenarioContext _scenarioContext;

    public TestHooks(ScenarioTestContext context, ScenarioContext scenarioContext)
    {
        _context = context;
        _scenarioContext = scenarioContext;
    }


    [BeforeScenario(Order = 10)]
    public async Task BeforeScenarioAsync()
    {
        if (_scenarioContext.ScenarioInfo.CombinedTags.Any(tag =>
                tag.Equals(TestTags.NoBrowser, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        _context.Driver = new BrowserDriver();
        await _context.Driver.StartAsync();

        var page = _context.Driver.Page;
        _context.LoginPage = new LoginPage(page);
        _context.NavigationPage = new NavigationPage(page);
        _context.DashboardPage = new DashboardPage(page);
        _context.MyProfilePage = new MyProfilePage(page);
        _context.LeaveApplicationPage = new LeaveApplicationPage(page);
        _context.AttendanceRecordPage = new AttendanceRecordPage(page);
        _context.LeaveCorrectionPage = new LeaveCorrectionPage(page);
        _context.EmployeeDirectoryPage = new EmployeeDirectoryPage(page);
        _context.FooterPage = new FooterPage(page);
        _context.LogoutPage = new LogoutPage(page);
    }

    [AfterScenario(Order = 100)]
    public async Task AfterScenarioAsync()
    {
        if (_context.Driver is not null)
        {
            var failedScenarioName = _scenarioContext.TestError is null
                ? null
                : _scenarioContext.ScenarioInfo.Title;
            var artifacts = await _context.Driver.StopAsync(failedScenarioName);
            foreach (var artifact in artifacts.Where(File.Exists))
                NUnit.Framework.TestContext.AddTestAttachment(artifact);
        }
    }
}
