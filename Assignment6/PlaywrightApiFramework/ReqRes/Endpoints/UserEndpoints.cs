namespace PlaywrightApiFramework.ReqRes.Endpoints;

public static class UserEndpoints
{
    public static string Users = "/api/users";
    public static string Register = "/api/register";

    public static string UsersPage(int page)
    {
        return "/api/users?page=" + page;
    }

    public static string SingleUser(int id)
    {
        return "/api/users/" + id;
    }
}
