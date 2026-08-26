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
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.SortBy(ProductSortOption.NameAscending);

        var names = await inventoryPage.GetProductNames();
        var expectedNames = names.OrderBy(name => name).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync(ProductSortOption.NameAscending.ToSelectValue());
        Assert.That(names, Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task SortProducts_ByNameDescending_ShouldDisplayDescendingNames()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.SortBy(ProductSortOption.NameDescending);

        var names = await inventoryPage.GetProductNames();
        var expectedNames = names.OrderByDescending(name => name).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync(ProductSortOption.NameDescending.ToSelectValue());
        Assert.That(names, Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task SortProducts_ByPriceLowToHigh_ShouldDisplayAscendingPrices()
    {
        var inventoryPage = await OpenAndLogin();

        await StepAsync("Sort products by price low to high", () =>
            inventoryPage.SortBy(ProductSortOption.PriceLowToHigh));

        var prices = await inventoryPage.GetProductPrices();
        var expectedPrices = prices.OrderBy(price => price).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync(ProductSortOption.PriceLowToHigh.ToSelectValue());
        Assert.That(prices, Is.EqualTo(expectedPrices));
    }

    [Test]
    public async Task SortProducts_ByPriceHighToLow_ShouldDisplayDescendingPrices()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.SortBy(ProductSortOption.PriceHighToLow);

        var prices = await inventoryPage.GetProductPrices();
        var expectedPrices = prices.OrderByDescending(price => price).ToList();

        await Expect(inventoryPage.SortDropdown).ToHaveValueAsync(ProductSortOption.PriceHighToLow.ToSelectValue());
        Assert.That(prices, Is.EqualTo(expectedPrices));
    }

    [Test]
    public async Task ProductPreview_WhenProductNameClicked_ShouldShowProductDetails()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.OpenProductDetails(ProductTestData.Backpack);

        var detailsPage = new ProductDetailsPage(Page);
        await Expect(detailsPage.Name).ToHaveTextAsync(ProductTestData.Backpack);
        await Expect(detailsPage.Description).ToContainTextAsync(ProductTestData.BackpackDescriptionExcerpt);
        await Expect(detailsPage.Price).ToHaveTextAsync(ProductTestData.BackpackPrice);
        await Expect(detailsPage.AddToCartButton).ToBeVisibleAsync();

        await detailsPage.BackToProducts();
        await Expect(inventoryPage.Title).ToHaveTextAsync(ExpectedText.InventoryTitle);
    }

    [Test]
    public async Task AddProducts_WithRandomSelection_ShouldUseDynamicLocators()
    {
        var inventoryPage = await OpenAndLogin();
        var selectedProducts = ProductTestData.GetRandomProducts();

        await StepAsync("Hover a selected product", () => inventoryPage.HoverProduct(selectedProducts[0]));
        await StepAsync($"Add {selectedProducts.Count} randomly selected products", () =>
            inventoryPage.AddProductsToCart(selectedProducts));

        await Expect(inventoryPage.CartBadge).ToHaveTextAsync(selectedProducts.Count.ToString());

        await inventoryPage.OpenCart();
        var cartPage = new CartPage(Page);

        foreach (var product in selectedProducts)
        {
            await Expect(cartPage.RowByProductName(product)).ToBeVisibleAsync();
        }
    }

    [Test]
    public async Task BrowserBack_FromProductDetails_ShouldReturnToInventory()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.OpenProductDetails(ProductTestData.Backpack);
        await Page.WaitForURLAsync(AppRoutes.ProductDetailsWaitPattern);

        await Page.GoBackAsync();

        await Expect(Page).ToHaveURLAsync(AppRoutes.InventoryUrlPattern);
        await Expect(inventoryPage.Title).ToHaveTextAsync(ExpectedText.InventoryTitle);
    }
}
