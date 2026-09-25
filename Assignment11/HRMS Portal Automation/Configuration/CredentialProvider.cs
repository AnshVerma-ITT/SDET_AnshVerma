namespace HRIntimeAutomation.Configuration;

public sealed record Credentials(string Username, string Password);

public static class CredentialProvider
{
    private const string UsernameVariable = "HRMS_USERNAME";
    private const string PasswordVariable = "HRMS_PASSWORD";

    public static Credentials GetRequiredAdminCredentials()
    {
        // Loads local .env values only when the environment
        // variables have not already been provided.
        LoadDotEnv();

        var username = Environment.GetEnvironmentVariable(UsernameVariable);
        var password = Environment.GetEnvironmentVariable(PasswordVariable);

        var missingVariables = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
            missingVariables.Add(UsernameVariable);

        if (string.IsNullOrWhiteSpace(password))
            missingVariables.Add(PasswordVariable);

        if (missingVariables.Count > 0)
        {
            throw new InvalidOperationException(
                $"Required authentication environment variable(s) are missing: " +
                $"{string.Join(", ", missingVariables)}. " +
                "Provide them through the local .env file or the CI/CD environment.");
        }

        return new Credentials(username!, password!);
    }

    private static void LoadDotEnv()
    {
        var envPath = FindDotEnv();

        if (envPath is null)
            return;

        foreach (var rawLine in File.ReadAllLines(envPath))
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            var separatorIndex = line.IndexOf('=');

            if (separatorIndex <= 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..]
                .Trim()
                .Trim('"', '\'');

            // Never overwrite an environment variable that has
            // already been supplied by the operating system,
            // Jenkins, or another CI/CD environment.
            if (Environment.GetEnvironmentVariable(key) is null)
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    private static string? FindDotEnv()
    {
        foreach (var startPath in new[]
                 {
                     Directory.GetCurrentDirectory(),
                     AppContext.BaseDirectory
                 })
        {
            var directory = new DirectoryInfo(startPath);

            while (directory is not null)
            {
                var candidate = Path.Combine(directory.FullName, ".env");

                if (File.Exists(candidate))
                    return candidate;

                directory = directory.Parent;
            }
        }

        return null;
    }
}