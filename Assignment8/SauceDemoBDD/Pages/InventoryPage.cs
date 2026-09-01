using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace SauceDemoBDD.Pages;

public sealed class InventoryPage
{
    private const string ProductCardsSelector = ".inventory_item";
    private const string ProductPriceSelector = ".inventory_item_price";
    private const string SortTestId = "product-sort-container";
    private const string CartLinkTestId = "shopping-cart-link";
    private const string CartBadgeTestId = "shopping-cart-badge";
    private const string PageTitleTestId = "title";
    private const string ProductNameTestId = "inventory-item-name";
    private const string PriceLowToHighValue = "lohi";
    private const string ExpectedPageTitle = "Products";

    private readonly IPage _page;

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    private ILocator ProductCards => _page.Locator(ProductCardsSelector);
    private ILocator SortDropdown => _page.GetByTestId(SortTestId);
    private ILocator CartLink => _page.GetByTestId(CartLinkTestId);
    private ILocator CartBadge => _page.GetByTestId(CartBadgeTestId);
    private ILocator PageTitle => _page.GetByTestId(PageTitleTestId);

    public async Task<bool> IsDisplayed()
    {
        return await PageTitle.IsVisibleAsync()
            && await PageTitle.InnerTextAsync() == ExpectedPageTitle;
    }

    public Task SortByPriceLowToHigh()
    {
        return SortDropdown.SelectOptionAsync(PriceLowToHighValue);
    }

    public async Task<IReadOnlyList<decimal>> GetProductPrices()
    {
        var prices = new List<decimal>();
        var count = await ProductCards.CountAsync();

        for (var index = 0; index < count; index++)
        {
            var price = await ProductCards.Nth(index).Locator(ProductPriceSelector).InnerTextAsync();
            prices.Add(decimal.Parse(price.TrimStart('$'), CultureInfo.InvariantCulture));
        }

        return prices;
    }

    public async Task AddProducts(IEnumerable<string> productNames)
    {
        foreach (var productName in productNames)
        {
            await GetProductCard(productName)
                .GetByRole(AriaRole.Button, new() { Name = "Add to cart" })
                .ClickAsync();
        }
    }

    public Task RemoveProduct(string productName)
    {
        return GetProductCard(productName)
            .GetByRole(AriaRole.Button, new() { Name = "Remove" })
            .ClickAsync();
    }

    public async Task<int> GetCartCount()
    {
        return await CartBadge.CountAsync() == 0
            ? 0
            : int.Parse(await CartBadge.InnerTextAsync(), CultureInfo.InvariantCulture);
    }

    public Task OpenCart()
    {
        return CartLink.ClickAsync();
    }

    private ILocator GetProductCard(string productName)
    {
        var exactProductName = _page
            .GetByTestId(ProductNameTestId)
            .Filter(new LocatorFilterOptions
            {
                HasTextRegex = new Regex($"^{Regex.Escape(productName)}$")
            });

        return ProductCards.Filter(new LocatorFilterOptions { Has = exactProductName });
    }
}
