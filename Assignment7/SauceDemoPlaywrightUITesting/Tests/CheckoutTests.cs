using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Fixtures;
using SauceDemo.Playwright.Tests.Pages;
using SauceDemo.Playwright.Tests.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemo.Playwright.Tests.Tests;

[TestFixtureSource(typeof(BrowserMatrix), nameof(BrowserMatrix.Engines))]
public sealed class CheckoutTests : TestBase
{
    public CheckoutTests(BrowserEngine browserEngine) : base(browserEngine)
    {
    }

    [Test]
    public async Task Checkout_CompletePurchaseFlow_ShouldSucceed()
    {
        var inventoryPage = await OpenAndLogin();

        await inventoryPage.SortBy(ProductSortOption.PriceLowToHigh);
        await inventoryPage.AddProductsToCart(ProductTestData.ProductsForCart);
        await inventoryPage.OpenCart();

        var cartPage = new CartPage(Page);
        foreach (var product in ProductTestData.ProductsForCart)
        {
            await Expect(cartPage.RowByProductName(product)).ToBeVisibleAsync();
        }

        await cartPage.ContinueShopping();
        var remainingProducts = ProductTestData.ProductsForCheckout
            .Except(ProductTestData.ProductsForCart)
            .ToArray();
        await inventoryPage.AddProductsToCart(remainingProducts);
        await inventoryPage.OpenCart();
        await cartPage.Checkout();

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillCustomerInformation(
            CheckoutTestData.FirstName,
            CheckoutTestData.LastName,
            CheckoutTestData.PostalCode);
        await checkoutPage.Continue();

        await Expect(checkoutPage.OverviewTitle).ToBeVisibleAsync();
        await Expect(checkoutPage.SummaryItems).ToHaveCountAsync(ProductTestData.ProductsForCheckout.Count);
        await Expect(checkoutPage.Subtotal).ToContainTextAsync(ExpectedText.CheckoutItemTotalLabel);
        await Expect(checkoutPage.Tax).ToContainTextAsync(ExpectedText.CheckoutTaxLabel);
        await Expect(checkoutPage.Total).ToContainTextAsync(ExpectedText.CheckoutTotalLabel);

        foreach (var product in ProductTestData.ProductsForCheckout)
        {
            await Expect(checkoutPage.SummaryItemByProductName(product)).ToBeVisibleAsync();
        }

        await checkoutPage.Finish();

        await Expect(checkoutPage.CompleteTitle).ToBeVisibleAsync();
        await Expect(checkoutPage.ConfirmationMessage).ToHaveTextAsync(ExpectedText.OrderConfirmation);
    }

    [TestCase("", CheckoutTestData.LastName, CheckoutTestData.PostalCode, ExpectedText.FirstNameRequiredError)]
    [TestCase(CheckoutTestData.FirstName, "", CheckoutTestData.PostalCode, ExpectedText.LastNameRequiredError)]
    [TestCase(CheckoutTestData.FirstName, CheckoutTestData.LastName, "", ExpectedText.PostalCodeRequiredError)]
    public async Task Checkout_WithMissingRequiredInformation_ShouldShowError(
        string firstName,
        string lastName,
        string postalCode,
        string expectedError)
    {
        var inventoryPage = await OpenAndLogin();
        await inventoryPage.AddProductsToCart([ProductTestData.Backpack]);
        await inventoryPage.OpenCart();

        var cartPage = new CartPage(Page);
        await cartPage.Checkout();

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillCustomerInformation(firstName, lastName, postalCode);
        await checkoutPage.Continue();

        await Expect(checkoutPage.ErrorMessage).ToBeVisibleAsync();
        await Expect(checkoutPage.ErrorMessage).ToContainTextAsync(expectedError);
    }
}
