using SauceDemoBDD.Pages;
using SauceDemoBDD.TestData;

namespace SauceDemoBDD.Flows;

public sealed class LoginFlow
{
    private readonly LoginPage _loginPage;

    public LoginFlow(LoginPage loginPage)
    {
        _loginPage = loginPage;
    }

    public async Task LoginAsStandardUser()
    {
        await _loginPage.Open();
        await _loginPage.Login(
            LoginTestData.StandardUsername,
            LoginTestData.StandardPassword);
    }
}
