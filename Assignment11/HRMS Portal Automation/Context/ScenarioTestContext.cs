using HRIntimeAutomation.Helper;
using HRIntimeAutomation.Models;
using HRIntimeAutomation.Pages;

namespace HRIntimeAutomation.Context;

public sealed class ScenarioTestContext
{
    public BrowserDriver Driver { get; set; } = null!;
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
    public bool HoldsLeaveOperationGate { get; set; }
    public AttendanceRangeSelection? AttendanceRange { get; set; }
    public IReadOnlyList<FooterLinkResult> FooterLinkResults { get; set; } = [];
    public LeaveApplicationResult? LeaveApplicationResult { get; set; }
    public LeaveCorrectionResult? LeaveCorrectionResult { get; set; }
    public List<NavigationResult> NavigationResults { get; } = [];
}
