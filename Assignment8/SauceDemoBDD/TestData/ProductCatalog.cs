namespace SauceDemoBDD.TestData;

public static class ProductCatalog
{
    private static readonly IReadOnlyDictionary<string, string> Products =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["backpack"] = "Sauce Labs Backpack",
            ["bike light"] = "Sauce Labs Bike Light",
            ["bolt t-shirt"] = "Sauce Labs Bolt T-Shirt",
            ["fleece jacket"] = "Sauce Labs Fleece Jacket",
            ["onesie"] = "Sauce Labs Onesie",
            ["red t-shirt"] = "Test.allTheThings() T-Shirt (Red)"
        };

    public static string GetName(string productKey)
    {
        return Products.TryGetValue(productKey, out var productName)
            ? productName
            : throw new ArgumentException($"Unknown product key: {productKey}");
    }
}
