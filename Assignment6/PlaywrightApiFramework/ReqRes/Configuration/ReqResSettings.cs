using PlaywrightApiFramework.Framework.Utilities;

namespace PlaywrightApiFramework.ReqRes.Configuration;

public class ReqResSettings
{
    public string EmailDomain { get; private set; } = "@reqres.in";

    public static ReqResSettings Load()
    {
        var settings = new ReqResSettings();
        settings.EmailDomain = EnvironmentHelper.GetValue("REQRES_EMAIL_DOMAIN", settings.EmailDomain);
        return settings;
    }
}
