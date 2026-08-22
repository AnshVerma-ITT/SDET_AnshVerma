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
    public async Task CompletePurchaseFlow_ShouldSucceed()
    {
        var inventoryPage = await OpenAndLoginAsync();

        await inventoryPage.SortByAsync("lohi");
        await inventoryPage.AddProductToCartAsync(ProductTestData.Backpack);
        await inventoryPage.AddProductToCartAsync(ProductTestData.BikeLight);
        await inventoryPage.OpenCartAsync();

        var cartPage = new CartPage(Page);
        foreach (var product in ProductTestData.ProductsForCart)
        {
            await Expect(cartPage.RowByProductName(product)).ToBeVisibleAsync();
        }

        await cartPage.ContinueShoppingAsync();
        await inventoryPage.AddProductToCartAsync(ProductTestData.BoltTShirt);
        await inventoryPage.OpenCartAsync();
        await cartPage.CheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillCustomerInformationAsync(
            CheckoutTestData.FirstName,
            CheckoutTestData.LastName,
            CheckoutTestData.PostalCode);
        await checkoutPage.ContinueAsync();

        await Expect(checkoutPage.OverviewTitle).ToBeVisibleAsync();
        await Expect(checkoutPage.SummaryItems).ToHaveCountAsync(ProductTestData.ProductsForCheckout.Length);
        await Expect(checkoutPage.Subtotal).ToContainTextAsync("Item total:");
        await Expect(checkoutPage.Tax).ToContainTextAsync("Tax:");
        await Expect(checkoutPage.Total).ToContainTextAsync("Total:");

        foreach (var product in ProductTestData.ProductsForCheckout)
        {
            await Expect(checkoutPage.SummaryItemByProductName(product)).ToBeVisibleAsync();
        }

        await checkoutPage.FinishAsync();

        await Expect(checkoutPage.CompleteTitle).ToBeVisibleAsync();
        await Expect(checkoutPage.ConfirmationMessage).ToHaveTextAsync("Thank you for your order!");
    }

    [TestCase("", CheckoutTestData.LastName, CheckoutTestData.PostalCode, "First Name is required")]
    [TestCase(CheckoutTestData.FirstName, "", CheckoutTestData.PostalCode, "Last Name is required")]
    [TestCase(CheckoutTestData.FirstName, CheckoutTestData.LastName, "", "Postal Code is required")]
    public async Task Checkout_WithMissingRequiredInformation_ShouldShowError(
        string firstName,
        string lastName,
        string postalCode,
        string expectedError)
    {
        var inventoryPage = await OpenAndLoginAsync();
        await inventoryPage.AddProductToCartAsync(ProductTestData.Backpack);
        await inventoryPage.OpenCartAsync();

        var cartPage = new CartPage(Page);
        await cartPage.CheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillCustomerInformationAsync(firstName, lastName, postalCode);
        await checkoutPage.ContinueAsync();

        await Expect(checkoutPage.ErrorMessage).ToBeVisibleAsync();
        await Expect(checkoutPage.ErrorMessage).ToContainTextAsync(expectedError);
    }
}
