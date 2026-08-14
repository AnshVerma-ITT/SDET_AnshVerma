namespace PlaywrightApiFramework.Framework.Authentication;

public static class AuthManager
{
    public static Dictionary<string, string> GetHeaders(string apiKey)
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" }
        };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers.Add("x-api-key", apiKey);
        }
        return headers;
    }
}
