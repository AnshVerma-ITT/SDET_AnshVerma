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

    private ILocator Title => _page.Locator(TitleSelector);
    private ILocator ProductRows => _page.Locator(RowsSelector);

    public Task<string> GetPageTitle()
    {
        return Title.InnerTextAsync();
    }

    public Task<int> GetProductRowCount()
    {
        return ProductRows.CountAsync();
    }

    public Task<bool> IsProductDisplayed(string productName)
    {
        return FindProductRowByName(productName).IsVisibleAsync();
    }

    public async Task<string> GetQuantityByProductName(string productName)
    {
        return await FindProductRowByName(productName).Locator(QuantitySelector).InnerTextAsync();
    }

    public async Task<string> GetProductPrice(string productName)
    {
        return await FindProductRowByName(productName).Locator(PriceSelector).InnerTextAsync();
    }

    public Task ClickOnContinueShoppingButton()
    {
        return _page.GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" }).ClickAsync();
    }

    public Task ClickOnCheckoutButton()
    {
        return _page.GetByRole(AriaRole.Button, new() { Name = "Checkout" }).ClickAsync();
    }

    private ILocator FindProductRowByName(string productName)
    {
        return ProductRows.Filter(new LocatorFilterOptions { HasText = productName });
    }
}
