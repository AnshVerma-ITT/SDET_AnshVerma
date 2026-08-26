using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class CartPage
{
    private const string TitleSelector = ".title";
    private const string RowsSelector = ".cart_item";
    private const string QuantitySelector = ".cart_quantity";
    private const string PriceSelector = ".inventory_item_price";

    private readonly IPage _page;

    public CartPage(IPage page)
    {
        _page = page;
    }

    public ILocator Title => _page.Locator(TitleSelector);
    public ILocator Rows => _page.Locator(RowsSelector);

    public ILocator RowByProductName(string productName)
    {
        return Rows.Filter(new LocatorFilterOptions { HasText = productName });
    }

    public async Task<string> GetQuantity(string productName)
    {
        return await RowByProductName(productName).Locator(QuantitySelector).InnerTextAsync();
    }

    public async Task<string> GetPriceText(string productName)
    {
        return await RowByProductName(productName).Locator(PriceSelector).InnerTextAsync();
    }

    public Task ContinueShopping()
    {
        return _page.GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" }).ClickAsync();
    }

    public Task Checkout()
    {
        return _page.GetByRole(AriaRole.Button, new() { Name = "Checkout" }).ClickAsync();
    }
}
