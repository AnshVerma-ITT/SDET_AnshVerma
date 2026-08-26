using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class ProductDetailsPage
{
    private const string NameSelector = ".inventory_details_name";
    private const string DescriptionSelector = ".inventory_details_desc";
    private const string PriceSelector = ".inventory_details_price";

    private readonly IPage _page;

    public ProductDetailsPage(IPage page)
    {
        _page = page;
    }

    public ILocator Name => _page.Locator(NameSelector);
    public ILocator Description => _page.Locator(DescriptionSelector);
    public ILocator Price => _page.Locator(PriceSelector);
    public ILocator AddToCartButton => _page.GetByRole(AriaRole.Button, new() { Name = "Add to cart" });
    public ILocator BackToProductsButton => _page.GetByRole(AriaRole.Button, new() { Name = "Back to products" });

    public Task BackToProducts()
    {
        return BackToProductsButton.ClickAsync();
    }
}
