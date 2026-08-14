using PlaywrightApiFramework.Framework.Utilities;

namespace PlaywrightApiFramework.Framework.Configuration;

public class AppSettings
{
    public string BaseUrl { get; set; } = PlaywrightConfig.DefaultBaseUrl;
    public string ApiKey { get; set; } = "reqres-free-v1";

    public static AppSettings Load()
    {
        var settings = new AppSettings();
        var envValues = ReadEnvFile();
        settings.BaseUrl = GetValue("REQRES_BASE_URL", settings.BaseUrl, envValues);
        settings.ApiKey = GetValue("REQRES_API_KEY", settings.ApiKey, envValues);
        return settings;
    }

    static string GetValue(string key, string defaultValue, Dictionary<string, string> envValues)
    {
        var systemValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(systemValue))
        {
            return systemValue;
        }
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
