namespace PlaywrightApiFramework.ReqRes.Endpoints;

public static class UserEndpoints
{
    public const string Users = "/api/users";
    public const string Register = "/api/register";

    public static string UsersPage(int page)
    {
        return $"{Users}?page={page}";
    }

    public static string SingleUser(int id)
    {
        return $"{Users}/{id}";
    }
}
