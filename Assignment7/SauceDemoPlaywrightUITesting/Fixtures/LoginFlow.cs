using Microsoft.Playwright;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Infrastructure;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;

namespace SauceDemo.Playwright.Tests.Fixtures;

public static class LoginFlow
{
    public static async Task<InventoryPage> OpenAndLogin(IPage page, TestSettings settings)
    {
        var loginPage = new LoginPage(page);
        await NavigationHelper.NavigateTo(page, AppRoutes.Root, settings);
        await loginPage.WaitUntilLoaded();
        await loginPage.Login(LoginTestData.ValidUsername, LoginTestData.ValidPassword);
        await page.WaitForURLAsync(AppRoutes.Inventory);

        return new InventoryPage(page);
    }
}
