namespace HRIntimeAutomation.TestData;

public static class LoginTestData
{
    // Deliberately fake values. These are not secrets and cannot authenticate.
    public const string InvalidUsername = "invalid.automation.user@intimetec.com";
    public const string InvalidPassword = "InvalidPassword!12345";
    public const string ExpectedInvalidLoginMessage = "Something went wrong!! Error processing request.";
    public const string DashboardUrlPath = "/user/dashboard";
}
