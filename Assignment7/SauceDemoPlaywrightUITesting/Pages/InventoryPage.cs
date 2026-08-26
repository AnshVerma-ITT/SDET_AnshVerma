using System.Globalization;
using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class InventoryPage
{
    private const string TitleSelector = ".title";
    private const string ProductItemsSelector = ".inventory_item";
    private const string SortDropdownSelector = ".product_sort_container";
    private const string CartBadgeSelector = ".shopping_cart_badge";
    private const string ProductNameSelector = ".inventory_item_name";
    private const string ShoppingCartLinkSelector = ".shopping_cart_link";
    private const string ProductPriceSelector = ".inventory_item_price";

    private readonly IPage _page;

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public ILocator Title => _page.Locator(TitleSelector);
    public ILocator ProductItems => _page.Locator(ProductItemsSelector);
    public ILocator SortDropdown => _page.Locator(SortDropdownSelector);
    public ILocator CartBadge => _page.Locator(CartBadgeSelector);

    public Task SortBy(ProductSortOption option)
    {
        return SortDropdown.SelectOptionAsync(option.ToSelectValue());
    }

    public async Task AddProductsToCart(IEnumerable<string> productNames)
    {
        foreach (var productName in productNames)
        {
            var product = ProductByName(productName);
            await product.GetByRole(AriaRole.Button, new() { Name = "Add to cart" }).ClickAsync();
        }
    }

    public Task HoverProduct(string productName)
    {
        return ProductByName(productName).HoverAsync();
    }

    public Task OpenProductDetails(string productName)
    {
        return ProductByName(productName).Locator(ProductNameSelector).ClickAsync();
    }

    public Task OpenCart()
    {
        return _page.Locator(ShoppingCartLinkSelector).ClickAsync();
    }

    public ILocator ProductByName(string productName)
    {
        return ProductItems.Filter(new LocatorFilterOptions { HasText = productName });
    }

    public async Task<IReadOnlyList<decimal>> GetProductPrices()
    {
        var prices = new List<decimal>();
        var count = await ProductItems.CountAsync();

        for (var index = 0; index < count; index++)
        {
            var text = await ProductItems.Nth(index).Locator(ProductPriceSelector).InnerTextAsync();
            prices.Add(decimal.Parse(text.Replace("$", string.Empty), CultureInfo.InvariantCulture));
        }

        return prices;
    }

    public async Task<IReadOnlyList<string>> GetProductNames()
    {
        var names = new List<string>();
        var count = await ProductItems.CountAsync();

        for (var index = 0; index < count; index++)
        {
            names.Add(await ProductItems.Nth(index).Locator(ProductNameSelector).InnerTextAsync());
        }

        return names;
    }
}
public enum ProductSortOption
{
    NameAscending,
    NameDescending,
    PriceLowToHigh,
    PriceHighToLow
}

public static class ProductSortOptionExtensions
{
    public static string ToSelectValue(this ProductSortOption option)
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
}