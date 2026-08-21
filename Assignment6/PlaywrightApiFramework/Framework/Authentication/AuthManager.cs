namespace PlaywrightApiFramework.Framework.Authentication;

public static class AuthManager
{
    public static Dictionary<string, string> GetHeaders(string headerName, string headerValue)
    {
        var headers = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(headerName) && !string.IsNullOrWhiteSpace(headerValue))
        {
            headers.Add(headerName, headerValue);
        }
        return headers;
    }
}
