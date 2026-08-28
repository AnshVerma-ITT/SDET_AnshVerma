using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class CartTests : TestBase
{
    public CartTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task Cart_WithSelectedProducts_ShouldReadRowsAndContinueShopping()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.AddProductsToCart(ProductTestData.ProductsForCart);

        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);
        var cartPage = new CartPage(Page);

        Assert.That(await cartPage.GetPageTitle(), Is.EqualTo(PageTitleTestData.Cart));
        Assert.That(await cartPage.GetProductRowCount(), Is.EqualTo(ProductTestData.ProductsForCart.Count));

        foreach (var product in ProductTestData.ProductsForCart)
        {
            Assert.That(await cartPage.IsProductDisplayed(product), Is.True);
            Assert.That(await cartPage.GetQuantityByProductName(product), Is.EqualTo(ProductTestData.DefaultCartQuantity));
            Assert.That(await cartPage.GetProductPrice(product), Does.StartWith(ProductTestData.CurrencySymbol));
        }

        await cartPage.ClickOnContinueShoppingButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Inventory);
        Assert.That(await inventoryPage.GetPageTitle(), Is.EqualTo(PageTitleTestData.Inventory));

        var remainingProducts = ProductTestData.ProductsForCheckout
            .Except(ProductTestData.ProductsForCart)
            .ToArray();
        await inventoryPage.AddProductsToCart(remainingProducts);
        Assert.That(
            await inventoryPage.GetCartProductCount(),
            Is.EqualTo(ProductTestData.ProductsForCheckout.Count));
    }

    [Test]
    public async Task Cart_WithNoProducts_ShouldBeEmpty()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.AddProductsToCart(Array.Empty<string>());
        Assert.That(await inventoryPage.GetCartProductCount(), Is.Zero);

        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);
        var cartPage = new CartPage(Page);

        Assert.That(await cartPage.GetPageTitle(), Is.EqualTo(PageTitleTestData.Cart));
        Assert.That(await cartPage.GetProductRowCount(), Is.Zero);
    }

    [Test]
    public async Task Cart_WithAllProducts_ShouldContainEveryProduct()
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.AddProductsToCart(ProductTestData.AllProducts);
        Assert.That(await inventoryPage.GetCartProductCount(), Is.EqualTo(ProductTestData.AllProducts.Count));

        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);
        var cartPage = new CartPage(Page);

        Assert.That(await cartPage.GetProductRowCount(), Is.EqualTo(ProductTestData.AllProducts.Count));
        foreach (var product in ProductTestData.AllProducts)
        {
            Assert.That(await cartPage.IsProductDisplayed(product), Is.True);
        }
    }
}
