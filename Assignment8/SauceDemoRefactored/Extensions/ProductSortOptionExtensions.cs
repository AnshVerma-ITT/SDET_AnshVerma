using SauceDemo.Playwright.Tests.Enums;

namespace SauceDemo.Playwright.Tests.Extensions;

public static class ProductSortOptionExtensions
{
    public static string ToDropdownValue(this ProductSortOption option)
    {
        return option switch
        {
            ProductSortOption.NameAscending => "az",
            ProductSortOption.NameDescending => "za",
            ProductSortOption.PriceLowToHigh => "lohi",
            ProductSortOption.PriceHighToLow => "hilo",
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Unsupported product sort option.")
        };
    }

    public static ProductSortOption ToProductSortOption(this string value)
    {
        return value switch
        {
            "az" => ProductSortOption.NameAscending,
            "za" => ProductSortOption.NameDescending,
            "lohi" => ProductSortOption.PriceLowToHigh,
            "hilo" => ProductSortOption.PriceHighToLow,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported product sort value.")
        };
    }
}
