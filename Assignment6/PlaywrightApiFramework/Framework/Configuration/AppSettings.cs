using PlaywrightApiFramework.Framework.Utilities;

namespace PlaywrightApiFramework.Framework.Configuration;

public class AppSettings
{
    public string BaseUrl { get; set; } = "";
    public string HeaderName { get; set; } = "";
    public string HeaderValue { get; set; } = "";

    public static AppSettings Load()
    {
        var settings = new AppSettings();
        settings.BaseUrl = EnvironmentHelper.GetValue("BASE_URL", settings.BaseUrl);
        settings.HeaderName = EnvironmentHelper.GetValue("HEADER_NAME", settings.HeaderName);
        settings.HeaderValue = EnvironmentHelper.GetValue("HEADER_VALUE", settings.HeaderValue);
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            throw new Exception("BASE_URL is missing in .env file.");
        }
        if (string.IsNullOrWhiteSpace(settings.HeaderName))
        {
            throw new Exception("HEADER_NAME is missing in .env file.");
        }
        if (string.IsNullOrWhiteSpace(settings.HeaderValue))
        {
            throw new Exception("HEADER_VALUE is missing in .env file.");
        }
        return settings;
    }
}
