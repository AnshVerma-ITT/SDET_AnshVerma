using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class ProductDetailsPage
{
    private readonly IPage _page;

    public ProductDetailsPage(IPage page)
    {
        _page = page;
    }

    public ILocator Name => _page.Locator(".inventory_details_name");
    public ILocator Description => _page.Locator(".inventory_details_desc");
    public ILocator Price => _page.Locator(".inventory_details_price");
    public ILocator AddToCartButton => _page.GetByRole(AriaRole.Button, new() { Name = "Add to cart" });
    public ILocator BackToProductsButton => _page.GetByRole(AriaRole.Button, new() { Name = "Back to products" });

    public Task BackToProductsAsync()
    {
        return BackToProductsButton.ClickAsync();
    }
}
