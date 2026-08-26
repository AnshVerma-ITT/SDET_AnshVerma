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
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.AddProductsToCart(ProductTestData.ProductsForCart);

        await inventoryPage.OpenCart();
        var cartPage = new CartPage(Page);

        await Expect(cartPage.Title).ToHaveTextAsync(ExpectedText.CartTitle);
        await Expect(cartPage.Rows).ToHaveCountAsync(ProductTestData.ProductsForCart.Count);

        foreach (var product in ProductTestData.ProductsForCart)
        {
            await Expect(cartPage.RowByProductName(product)).ToBeVisibleAsync();
            Assert.That(await cartPage.GetQuantity(product), Is.EqualTo(ProductTestData.DefaultCartQuantity));
            Assert.That(await cartPage.GetPriceText(product), Does.StartWith(ProductTestData.CurrencySymbol));
        }

        await cartPage.ContinueShopping();
        await Expect(inventoryPage.Title).ToHaveTextAsync(ExpectedText.InventoryTitle);

        var remainingProducts = ProductTestData.ProductsForCheckout
            .Except(ProductTestData.ProductsForCart)
            .ToArray();
        await inventoryPage.AddProductsToCart(remainingProducts);
        await Expect(inventoryPage.CartBadge).ToHaveTextAsync(ProductTestData.ProductsForCheckout.Count.ToString());
    }

    [Test]
    public async Task Cart_WithNoProducts_ShouldBeEmpty()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.AddProductsToCart(Array.Empty<string>());
        await Expect(inventoryPage.CartBadge).ToHaveCountAsync(0);

        await inventoryPage.OpenCart();
        var cartPage = new CartPage(Page);

        await Expect(cartPage.Title).ToHaveTextAsync(ExpectedText.CartTitle);
        await Expect(cartPage.Rows).ToHaveCountAsync(0);
    }

    [Test]
    public async Task Cart_WithAllProducts_ShouldContainEveryProduct()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.AddProductsToCart(ProductTestData.AllProducts);
        await Expect(inventoryPage.CartBadge).ToHaveTextAsync(ProductTestData.AllProducts.Count.ToString());

        await inventoryPage.OpenCart();
        var cartPage = new CartPage(Page);

        await Expect(cartPage.Rows).ToHaveCountAsync(ProductTestData.AllProducts.Count);
        foreach (var product in ProductTestData.AllProducts)
        {
            await Expect(cartPage.RowByProductName(product)).ToBeVisibleAsync();
        }
    }
}
