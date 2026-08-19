using PlaywrightApiFramework.Framework.Utilities;

namespace PlaywrightApiFramework.Framework.Configuration;

public class AppSettings
{
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string AuthHeaderName { get; set; } = "";

    public static AppSettings Load()
    {
        var settings = new AppSettings();
        settings.BaseUrl = EnvironmentHelper.GetValue("BASE_URL", settings.BaseUrl);
        settings.ApiKey = EnvironmentHelper.GetValue("API_KEY", settings.ApiKey);
        settings.AuthHeaderName = EnvironmentHelper.GetValue("AUTH_HEADER_NAME", settings.AuthHeaderName);
        return settings;
    }
}
