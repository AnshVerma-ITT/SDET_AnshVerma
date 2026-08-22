namespace SauceDemo.Playwright.Tests.TestData;

public static class ProductTestData
{
    public const string Backpack = "Sauce Labs Backpack";
    public const string BikeLight = "Sauce Labs Bike Light";
    public const string BoltTShirt = "Sauce Labs Bolt T-Shirt";

    public static readonly string[] ProductsForCart =
    [
        Backpack,
        BikeLight
    ];

    public static readonly string[] ProductsForCheckout =
    [
        Backpack,
        BikeLight,
        BoltTShirt
    ];
}
