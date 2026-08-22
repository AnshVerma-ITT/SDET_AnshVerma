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
    public async Task Cart_ShouldReadRowsAndContinueShopping()
    {
        var inventoryPage = await OpenAndLoginAsync();

        foreach (var product in ProductTestData.ProductsForCart)
        {
            await inventoryPage.AddProductToCartAsync(product);
        }

        await inventoryPage.OpenCartAsync();
        var cartPage = new CartPage(Page);

        await Expect(cartPage.Title).ToHaveTextAsync("Your Cart");
        await Expect(cartPage.Rows).ToHaveCountAsync(ProductTestData.ProductsForCart.Length);

        foreach (var product in ProductTestData.ProductsForCart)
        {
            await Expect(cartPage.RowByProductName(product)).ToBeVisibleAsync();
            Assert.That(await cartPage.GetQuantityAsync(product), Is.EqualTo("1"));
            Assert.That(await cartPage.GetPriceTextAsync(product), Does.StartWith("$"));
        }

        await cartPage.ContinueShoppingAsync();
        await Expect(inventoryPage.Title).ToHaveTextAsync("Products");

        await inventoryPage.AddProductToCartAsync(ProductTestData.BoltTShirt);
        await Expect(inventoryPage.CartBadge).ToHaveTextAsync("3");
    }
}
