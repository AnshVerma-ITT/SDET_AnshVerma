namespace PlaywrightApiFramework.Framework.Utilities;

public static class EnvironmentHelper
{
    public static string GetValue(string key, string defaultValue)
    {
        var systemValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(systemValue))
        {
            return systemValue;
        }
        var envValues = ReadEnvFile();
        if (envValues.ContainsKey(key) && !string.IsNullOrWhiteSpace(envValues[key]))
        {
            return envValues[key];
        }
        return defaultValue;
    }

    static Dictionary<string, string> ReadEnvFile()
    {
        var values = new Dictionary<string, string>();
        var envPath = FileHelper.FindFileOrEmpty(".env");
        if (envPath == "")
        {
            return values;
        }
        foreach (var line in File.ReadAllLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
            {
                continue;
            }
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                values[parts[0].Trim()] = parts[1].Trim();
            }
        }
        return values;
    }
}
