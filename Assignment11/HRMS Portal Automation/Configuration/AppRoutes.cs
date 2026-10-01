namespace HRIntimeAutomation.Configuration;

public static class AppRoutes
{
    public const string Login = "/?redirectURL=/login";
    public const string Dashboard = "/user/dashboard";
    public const string MyProfile = "/user/organization/my-profile?tab=myDetails&verticaltab=personalDetail";
    public const string EmployeeDirectory = "/user/organization/employee-directory";
    public const string AttendanceRecord = "/user/leave/attendance-record";
    public const string LeavesApplication = "/user/leave/leaves-application";
    public const string LeaveEntitlements = "/user/leave/leave-entitlements?tab=myEntitlementRequests";
    public const string LeaveCorrection = "/user/leave/leave-correction";
    public const string MyHolidays = "/user/leave/my-holidays";

    public static readonly IReadOnlyList<string> ProtectedRoutes =
    [
        Dashboard,
        MyProfile,
        EmployeeDirectory,
        AttendanceRecord,
        LeavesApplication,
        LeaveEntitlements,
        LeaveCorrection,
        MyHolidays
    ];
}
