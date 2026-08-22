using System.Globalization;
using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class InventoryPage
{
    private readonly IPage _page;

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public ILocator Title => _page.Locator(".title");
    public ILocator ProductItems => _page.Locator(".inventory_item");
    public ILocator SortDropdown => _page.Locator(".product_sort_container");
    public ILocator CartBadge => _page.Locator(".shopping_cart_badge");

    public Task SortByAsync(string value)
    {
        return SortDropdown.SelectOptionAsync(value);
    }

    public async Task AddProductToCartAsync(string productName)
    {
        var product = ProductByName(productName);
        await product.GetByRole(AriaRole.Button, new() { Name = "Add to cart" }).ClickAsync();
    }

    public Task HoverProductAsync(string productName)
    {
        return ProductByName(productName).HoverAsync();
    }

    public Task OpenProductDetailsAsync(string productName)
    {
        return ProductByName(productName).Locator(".inventory_item_name").ClickAsync();
    }

    public Task OpenCartAsync()
    {
        return _page.Locator(".shopping_cart_link").ClickAsync();
    }

    public ILocator ProductByName(string productName)
    {
        return ProductItems.Filter(new LocatorFilterOptions { HasText = productName });
    }

    public async Task<IReadOnlyList<decimal>> GetProductPricesAsync()
    {
        var prices = new List<decimal>();
        var count = await ProductItems.CountAsync();

        for (var index = 0; index < count; index++)
        {
            var text = await ProductItems.Nth(index).Locator(".inventory_item_price").InnerTextAsync();
            prices.Add(decimal.Parse(text.Replace("$", string.Empty), CultureInfo.InvariantCulture));
        }

        return prices;
    }

    public async Task<IReadOnlyList<string>> GetProductNamesAsync()
    {
        var names = new List<string>();
        var count = await ProductItems.CountAsync();

        for (var index = 0; index < count; index++)
        {
            names.Add(await ProductItems.Nth(index).Locator(".inventory_item_name").InnerTextAsync());
        }

        return names;
    }
}
