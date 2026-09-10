using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace SauceDemoBDD.Pages;

public sealed class CartPage
{
    private const string CartRowsSelector = ".cart_item";
    private const string ProductNameTestId = "inventory-item-name";
    private const string ContinueShoppingTestId = "continue-shopping";
    private const string CheckoutTestId = "checkout";

    private readonly IPage _page;

    public CartPage(IPage page)
    {
        _page = page;
    }

    private ILocator CartRows => _page.Locator(CartRowsSelector);
    private ILocator ContinueShoppingButton => _page.GetByTestId(ContinueShoppingTestId);
    private ILocator CheckoutButton => _page.GetByTestId(CheckoutTestId);

    public Task<bool> ContainsProduct(string productName)
    {
        var exactProductName = _page
            .GetByTestId(ProductNameTestId)
            .Filter(new LocatorFilterOptions
            {
                HasTextRegex = new Regex($"^{Regex.Escape(productName)}$")
            });

        return CartRows
            .Filter(new LocatorFilterOptions { Has = exactProductName })
            .IsVisibleAsync();
    }

    public Task<int> GetProductCount()
    {
        return CartRows.CountAsync();
    }

    public Task ContinueShopping()
    {
        return ContinueShoppingButton.ClickAsync();
    }

    public Task StartCheckout()
    {
        return CheckoutButton.ClickAsync();
    }
}
