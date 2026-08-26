namespace SauceDemo.Playwright.Tests.TestData;

public static class ProductTestData
{
    public const string BrandName = "Sauce Labs";
    public const string Backpack = "Sauce Labs Backpack";
    public const string BikeLight = "Sauce Labs Bike Light";
    public const string BoltTShirt = "Sauce Labs Bolt T-Shirt";
    public const string FleeceJacket = "Sauce Labs Fleece Jacket";
    public const string Onesie = "Sauce Labs Onesie";
    public const string RedTShirt = "Test.allTheThings() T-Shirt (Red)";
    public const string BackpackDescriptionExcerpt = "carry.allTheThings()";
    public const string BackpackPrice = "$29.99";
    public const string DefaultCartQuantity = "1";
    public const string CurrencySymbol = "$";

    public static readonly IReadOnlyList<string> AllProducts = Array.AsReadOnly(
    [
        Backpack,
        BikeLight,
        BoltTShirt,
        FleeceJacket,
        Onesie,
        RedTShirt
    ]);

    public static readonly IReadOnlyList<string> ProductsForCart = Array.AsReadOnly(
    [
        Backpack,
        BikeLight
    ]);

    public static readonly IReadOnlyList<string> ProductsForCheckout = Array.AsReadOnly(
    [
        Backpack,
        BikeLight,
        BoltTShirt
    ]);

    public static IReadOnlyList<string> GetRandomProducts()
    {
        var products = AllProducts.ToArray();
        Random.Shared.Shuffle(products);
        var randomCount = Random.Shared.Next(1, AllProducts.Count + 1);

        return products[..randomCount];
    }
}
