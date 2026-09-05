namespace SauceDemoBDD.TestData;

public sealed record LoginCase(string Username, string Password, string ExpectedError);

public static class LoginTestData
{
    public const string StandardUsername = "standard_user";
    public const string StandardPassword = "secret_sauce";

    public static LoginCase GetCase(string caseName)
    {
        return caseName.ToLowerInvariant() switch
        {
            "invalid username" => new(
                "invalid_user",
                StandardPassword,
                "Username and password do not match"),
            "invalid password" => new(
                StandardUsername,
                "invalid_password",
                "Username and password do not match"),
            "missing username" => new(
                string.Empty,
                StandardPassword,
                "Username is required"),
            "missing password" => new(
                StandardUsername,
                string.Empty,
                "Password is required"),
            "locked user" => new(
                "locked_out_user",
                StandardPassword,
                "Sorry, this user has been locked out"),
            _ => throw new ArgumentException($"Unknown login case: {caseName}")
        };
    }
}
