namespace HRIntimeAutomation.TestData;

public static class LoginTestData
{
    public const string ValidCredentialToken = "valid";
    public const string InvalidCredentialToken = "invalid";
    public const string BlankCredentialToken = "blank";
    public const string ValidWithSpacesCredentialToken = "validWithSpaces";
    public const string DashboardResult = "dashboard";
    public const string ErrorResult = "error";
    public const string ValidationResult = "validation";
    // Deliberately fake values. These are not secrets and cannot authenticate.
    public const string InvalidUsername = "invalid.automation.user@intimetec.com";
    public const string InvalidPassword = "InvalidPassword!12345";
    public const string ExpectedInvalidLoginMessage = "Something went wrong!! Error processing request.";
    public const string DashboardUrlPath = Configuration.AppRoutes.Dashboard;
}
