using System.Globalization;
using Microsoft.Playwright;
using SauceDemo.Playwright.Tests.Enums;
using SauceDemo.Playwright.Tests.Extensions;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class InventoryPage
{
    private const string TitleSelector = ".title";
    private const string ProductCardsSelector = ".inventory_item";
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

    private ILocator Title => _page.Locator(TitleSelector);
    private ILocator ProductCards => _page.Locator(ProductCardsSelector);
    private ILocator SortDropdown => _page.Locator(SortDropdownSelector);
    private ILocator CartBadge => _page.Locator(CartBadgeSelector);

    public Task SelectProductSortOption(ProductSortOption option)
    {
        return SortDropdown.SelectOptionAsync(option.ToDropdownValue());
    }

    public async Task AddProductsToCart(IEnumerable<string> productNames)
    {
        foreach (var productName in productNames)
        {
            var product = FindProductCardByName(productName);
            await product.GetByRole(AriaRole.Button, new() { Name = "Add to cart" }).ClickAsync();
        }
    }

    public Task HoverToProductCard(string productName)
    {
        return FindProductCardByName(productName).HoverAsync();
    }

    public Task OpenProductDetails(string productName)
    {
        return FindProductCardByName(productName).Locator(ProductNameSelector).ClickAsync();
    }

    public Task ClickOnShoppingCartLink()
    {
        return _page.Locator(ShoppingCartLinkSelector).ClickAsync();
    }

    public Task<string> GetPageTitle()
    {
        return Title.InnerTextAsync();
    }

    public async Task<ProductSortOption> GetSelectedProductSortOption()
    {
        var selectedValue = await SortDropdown.InputValueAsync();
        return selectedValue.ToProductSortOption();
    }

    public async Task<int> GetCartProductCount()
    {
        if (await CartBadge.CountAsync() == 0)
        {
            return 0;
        }

        var badgeText = await CartBadge.InnerTextAsync();
        return int.Parse(badgeText, CultureInfo.InvariantCulture);
    }

    public async Task<IReadOnlyList<decimal>> GetAllProductPrices()
    {
        var prices = new List<decimal>();
        var count = await ProductCards.CountAsync();

        for (var index = 0; index < count; index++)
        {
            var text = await ProductCards.Nth(index).Locator(ProductPriceSelector).InnerTextAsync();
            prices.Add(decimal.Parse(text.Replace("$", string.Empty), CultureInfo.InvariantCulture));
        }

        return prices;
    }

    public async Task<IReadOnlyList<string>> GetAllProductNames()
    {
        var names = new List<string>();
        var count = await ProductCards.CountAsync();

        for (var index = 0; index < count; index++)
        {
            names.Add(await ProductCards.Nth(index).Locator(ProductNameSelector).InnerTextAsync());
        }

        return names;
    }

    private ILocator FindProductCardByName(string productName)
    {
        return ProductCards.Filter(new LocatorFilterOptions { HasText = productName });
    }
}
