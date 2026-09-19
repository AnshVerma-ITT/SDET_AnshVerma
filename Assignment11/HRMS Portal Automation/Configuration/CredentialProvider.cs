namespace HRIntimeAutomation.Configuration;

public sealed record Credentials(string Username, string Password);

public static class CredentialProvider
{
    private const string UsernameVariable = "HRMS_USERNAME";
    private const string PasswordVariable = "HRMS_PASSWORD";

    public static Credentials GetRequiredAdminCredentials()
    {
        var username = Environment.GetEnvironmentVariable(UsernameVariable);
        var password = Environment.GetEnvironmentVariable(PasswordVariable);

        var missingVariables = new List<string>();
        if (string.IsNullOrWhiteSpace(username))
        {
            missingVariables.Add(UsernameVariable);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            missingVariables.Add(PasswordVariable);
        }

        if (missingVariables.Count > 0)
        {
            throw new InvalidOperationException(
                $"Set the following environment variables before running authenticated scenarios: {string.Join(", ", missingVariables)}. " +
                "Never store real HRINTIME credentials in source code or feature files.");
        }

        return new Credentials(username!, password!);
    }
}
