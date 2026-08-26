using System.Text.RegularExpressions;

namespace SauceDemo.Playwright.Tests.Configuration;

public static class AppRoutes
{
    public const string Root = "/";
    public const string InventoryPath = "/inventory.html";
    public const string InventoryWaitPattern = "**/inventory.html";
    public const string ProductDetailsWaitPattern = "**/inventory-item.html?id=*";
    public static readonly Regex InventoryUrlPattern = new($@".*{Regex.Escape(InventoryPath)}$");
}
