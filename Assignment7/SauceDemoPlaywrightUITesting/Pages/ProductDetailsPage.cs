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

    private ILocator Name => _page.Locator(NameSelector);
    private ILocator Description => _page.Locator(DescriptionSelector);
    private ILocator Price => _page.Locator(PriceSelector);
    private ILocator AddToCartButton => _page.GetByRole(AriaRole.Button, new() { Name = "Add to cart" });
    private ILocator BackToProductsButton => _page.GetByRole(AriaRole.Button, new() { Name = "Back to products" });

    public Task<string> GetProductName()
    {
        return Name.InnerTextAsync();
    }

    public Task<string> GetProductDescription()
    {
        return Description.InnerTextAsync();
    }

    public Task<string> GetProductPrice()
    {
        return Price.InnerTextAsync();
    }

    public Task<bool> IsAddToCartButtonDisplayed()
    {
        return AddToCartButton.IsVisibleAsync();
    }

    public Task ClickOnBackToProductsButton()
    {
        return BackToProductsButton.ClickAsync();
    }
}
