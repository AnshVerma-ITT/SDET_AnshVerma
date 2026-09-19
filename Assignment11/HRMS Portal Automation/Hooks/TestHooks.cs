using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.Helper;
using HRIntimeAutomation.Pages;
using Reqnroll;

namespace HRIntimeAutomation.Hooks;

[Binding]
public sealed class TestHooks
{
    // Leave Application and Leave Correction mutate the same employee account.
    // Keep only those scenarios serialized while the rest of the suite remains
    // eligible for NUnit fixture-level parallel execution.
    private static readonly SemaphoreSlim LeaveOperationGate = new(1, 1);

    private readonly ScenarioTestContext _context;
    private readonly ScenarioContext _scenarioContext;

    public TestHooks(ScenarioTestContext context, ScenarioContext scenarioContext)
    {
        _context = context;
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario(Order = 0)]
    public async Task SerializeAccountMutatingLeaveScenariosAsync()
    {
        var tags = _scenarioContext.ScenarioInfo.CombinedTags;
        var isMutatingLeaveScenario = tags.Any(tag =>
            tag.Equals(TestTags.Leave, StringComparison.OrdinalIgnoreCase) ||
            tag.Equals(TestTags.LeaveCorrection, StringComparison.OrdinalIgnoreCase));

        if (!isMutatingLeaveScenario)
            return;

        await LeaveOperationGate.WaitAsync();
        _context.HoldsLeaveOperationGate = true;
    }

    [BeforeScenario(Order = 10)]
    public async Task BeforeScenarioAsync()
    {
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
        try
        {
            if (_context.Driver is not null)
                await _context.Driver.StopAsync();
        }
        finally
        {
            if (_context.HoldsLeaveOperationGate)
            {
                _context.HoldsLeaveOperationGate = false;
                LeaveOperationGate.Release();
            }
        }
    }
}
