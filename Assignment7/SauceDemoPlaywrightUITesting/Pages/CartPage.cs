using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class CartPage
{
    private readonly IPage _page;

    public CartPage(IPage page)
    {
        _page = page;
    }

    public ILocator Title => _page.Locator(".title");
    public ILocator Rows => _page.Locator(".cart_item");

    public ILocator RowByProductName(string productName)
    {
        return Rows.Filter(new LocatorFilterOptions { HasText = productName });
    }

    public async Task<string> GetQuantityAsync(string productName)
    {
        return await RowByProductName(productName).Locator(".cart_quantity").InnerTextAsync();
    }

    public async Task<string> GetPriceTextAsync(string productName)
    {
        return await RowByProductName(productName).Locator(".inventory_item_price").InnerTextAsync();
    }

    public Task ContinueShoppingAsync()
    {
        return _page.GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" }).ClickAsync();
    }

    public Task CheckoutAsync()
    {
        return _page.GetByRole(AriaRole.Button, new() { Name = "Checkout" }).ClickAsync();
    }
}
