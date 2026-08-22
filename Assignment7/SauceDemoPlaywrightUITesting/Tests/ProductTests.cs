using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class ProductTests : TestBase
{
    public ProductTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task SortProducts_ByNameAscending_ShouldDisplayAscendingNames()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await inventoryPage.SortByAsync("az");

        var names = await inventoryPage.GetProductNamesAsync();
        var expectedNames = names.OrderBy(name => name).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync("az");
        Assert.That(names, Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task SortProducts_ByNameDescending_ShouldDisplayDescendingNames()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await inventoryPage.SortByAsync("za");

        var names = await inventoryPage.GetProductNamesAsync();
        var expectedNames = names.OrderByDescending(name => name).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync("za");
        Assert.That(names, Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task SortProducts_ByPriceLowToHigh_ShouldDisplayAscendingPrices()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await StepAsync("Sort products by price low to high", () => inventoryPage.SortByAsync("lohi"));

        var prices = await inventoryPage.GetProductPricesAsync();
        var expectedPrices = prices.OrderBy(price => price).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync("lohi");
        Assert.That(prices, Is.EqualTo(expectedPrices));
    }

    [Test]
    public async Task SortProducts_ByPriceHighToLow_ShouldDisplayDescendingPrices()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await inventoryPage.SortByAsync("hilo");

        var prices = await inventoryPage.GetProductPricesAsync();
        var expectedPrices = prices.OrderByDescending(price => price).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync("hilo");
        Assert.That(prices, Is.EqualTo(expectedPrices));
    }

    [Test]
    public async Task ProductPreview_WhenProductNameClicked_ShouldShowProductDetails()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await inventoryPage.OpenProductDetailsAsync(ProductTestData.Backpack);

        var detailsPage = new ProductDetailsPage(Page);
        await Expect(detailsPage.Name).ToHaveTextAsync(ProductTestData.Backpack);
        await Expect(detailsPage.Description).ToContainTextAsync("carry.allTheThings()");
        await Expect(detailsPage.Price).ToHaveTextAsync("$29.99");
        await Expect(detailsPage.AddToCartButton).ToBeVisibleAsync();

        await detailsPage.BackToProductsAsync();
        await Expect(inventoryPage.Title).ToHaveTextAsync("Products");
    }

    [Test]
    public async Task AddProducts_ByName_ShouldUseDynamicLocators()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await StepAsync("Hover the first product", () => inventoryPage.HoverProductAsync(ProductTestData.Backpack));

        foreach (var product in ProductTestData.ProductsForCart)
        {
            await StepAsync($"Add product: {product}", () => inventoryPage.AddProductToCartAsync(product));
        }

        await Expect(inventoryPage.CartBadge).ToHaveTextAsync(ProductTestData.ProductsForCart.Length.ToString());
    }
}
