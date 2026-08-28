using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Enums;
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
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.SelectProductSortOption(ProductSortOption.NameAscending);

        var names = await inventoryPage.GetAllProductNames();
        var expectedNames = names.OrderBy(name => name).ToList();

        Assert.That(await inventoryPage.GetSelectedProductSortOption(), Is.EqualTo(ProductSortOption.NameAscending));
        Assert.That(names, Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task SortProducts_ByNameDescending_ShouldDisplayDescendingNames()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.SelectProductSortOption(ProductSortOption.NameDescending);

        var names = await inventoryPage.GetAllProductNames();
        var expectedNames = names.OrderByDescending(name => name).ToList();

        Assert.That(await inventoryPage.GetSelectedProductSortOption(), Is.EqualTo(ProductSortOption.NameDescending));
        Assert.That(names, Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task SortProducts_ByPriceLowToHigh_ShouldDisplayAscendingPrices()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await AllureHelper.Step("Sort products by price low to high", () =>
            inventoryPage.SelectProductSortOption(ProductSortOption.PriceLowToHigh));

        var prices = await inventoryPage.GetAllProductPrices();
        var expectedPrices = prices.OrderBy(price => price).ToList();

        Assert.That(await inventoryPage.GetSelectedProductSortOption(), Is.EqualTo(ProductSortOption.PriceLowToHigh));
        Assert.That(prices, Is.EqualTo(expectedPrices));
    }

    [Test]
    public async Task SortProducts_ByPriceHighToLow_ShouldDisplayDescendingPrices()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.SelectProductSortOption(ProductSortOption.PriceHighToLow);

        var prices = await inventoryPage.GetAllProductPrices();
        var expectedPrices = prices.OrderByDescending(price => price).ToList();

        Assert.That(await inventoryPage.GetSelectedProductSortOption(), Is.EqualTo(ProductSortOption.PriceHighToLow));
        Assert.That(prices, Is.EqualTo(expectedPrices));
    }

    [Test]
    public async Task ProductPreview_WhenProductNameClicked_ShouldShowProductDetails()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.OpenProductDetails(ProductTestData.Backpack);
        await Expect(Page).ToHaveURLAsync(AppRoutes.GetProductDetailsRoute(ProductTestData.BackpackId));

        var detailsPage = new ProductDetailsPage(Page);
        Assert.That(await detailsPage.GetProductName(), Is.EqualTo(ProductTestData.Backpack));
        Assert.That(await detailsPage.GetProductDescription(), Does.Contain(ProductTestData.BackpackDescriptionExcerpt));
        Assert.That(await detailsPage.GetProductPrice(), Is.EqualTo(ProductTestData.BackpackPrice));
        Assert.That(await detailsPage.IsAddToCartButtonDisplayed(), Is.True);

        await detailsPage.ClickOnBackToProductsButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Inventory);
        Assert.That(await inventoryPage.GetPageTitle(), Is.EqualTo(PageTitleTestData.Inventory));
    }

    [Test]
    public async Task AddProducts_WithRandomSelection_ShouldUseDynamicLocators()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);
        var selectedProducts = ProductTestData.GetRandomProducts(Settings.RandomSeed);
        TestContext.Progress.WriteLine(
            $"Random product seed: {Settings.RandomSeed}; selected products: {string.Join(", ", selectedProducts)}");

        await AllureHelper.Step("Hover a selected product", () => inventoryPage.HoverToProductCard(selectedProducts[0]));
        await AllureHelper.Step($"Add {selectedProducts.Count} randomly selected products", () =>
            inventoryPage.AddProductsToCart(selectedProducts));

        Assert.That(await inventoryPage.GetCartProductCount(), Is.EqualTo(selectedProducts.Count));

        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);
        var cartPage = new CartPage(Page);

        foreach (var product in selectedProducts)
        {
            Assert.That(await cartPage.IsProductDisplayed(product), Is.True);
        }
    }

    [Test]
    public async Task BrowserBack_FromProductDetails_ShouldReturnToInventory()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.OpenProductDetails(ProductTestData.Backpack);
        await Expect(Page).ToHaveURLAsync(AppRoutes.GetProductDetailsRoute(ProductTestData.BackpackId));

        await Page.GoBackAsync();

        await Expect(Page).ToHaveURLAsync(AppRoutes.Inventory);
        Assert.That(await inventoryPage.GetPageTitle(), Is.EqualTo(PageTitleTestData.Inventory));
    }
}
