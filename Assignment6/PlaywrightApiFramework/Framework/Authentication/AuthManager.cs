using PlaywrightApiFramework.Framework.Constants;

namespace PlaywrightApiFramework.Framework.Authentication;

public static class AuthManager
{
    public static Dictionary<string, string> GetHeaders(string authHeaderName, string apiKey)
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", ApiConstants.ApplicationJson }
        };
        if (!string.IsNullOrWhiteSpace(authHeaderName) && !string.IsNullOrWhiteSpace(apiKey))
        {
            headers.Add(authHeaderName, apiKey);
        }
        return headers;
    }
}
