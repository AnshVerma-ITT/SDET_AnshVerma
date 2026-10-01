using HRIntimeAutomation.Models;
using HRIntimeAutomation.Pages;
using HRIntimeAutomation.Utilities;

namespace HRIntimeAutomation.Context;

public sealed class ScenarioTestContext
{
    public BrowserDriver? Driver { get; set; }
    public LoginPage LoginPage { get; set; } = null!;
    public NavigationPage NavigationPage { get; set; } = null!;
    public DashboardPage DashboardPage { get; set; } = null!;
    public MyProfilePage MyProfilePage { get; set; } = null!;
    public LeaveApplicationPage LeaveApplicationPage { get; set; } = null!;
    public AttendanceRecordPage AttendanceRecordPage { get; set; } = null!;
    public LeaveCorrectionPage LeaveCorrectionPage { get; set; } = null!;
    public EmployeeDirectoryPage EmployeeDirectoryPage { get; set; } = null!;
    public FooterPage FooterPage { get; set; } = null!;
    public LogoutPage LogoutPage { get; set; } = null!;
    public IReadOnlyList<DateTime> AttendanceDates { get; set; } = [];
    public IReadOnlyList<FooterLinkResult> FooterLinkResults { get; set; } = [];
    public LeaveApplicationResult? LeaveApplicationResult { get; set; }
    public LeaveCorrectionResult? LeaveCorrectionResult { get; set; }
    public List<NavigationResult> NavigationResults { get; } = [];
    public bool OrganizationWasCollapsed { get; set; }
    public bool OrganizationWasReopened { get; set; }
    public IReadOnlyDictionary<string, bool> SessionSecurityChecks { get; set; } =
        new Dictionary<string, bool>();
}
