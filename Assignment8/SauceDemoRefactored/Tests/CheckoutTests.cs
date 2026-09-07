using NUnit.Framework;
using SauceDemo.Playwright.Tests.Configuration;
using SauceDemo.Playwright.Tests.Enums;
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
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);

        await inventoryPage.SelectProductSortOption(ProductSortOption.PriceLowToHigh);
        await inventoryPage.AddProductsToCart(ProductTestData.ProductsForCart);
        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);

        var cartPage = new CartPage(Page);
        foreach (var product in ProductTestData.ProductsForCart)
        {
            Assert.That(await cartPage.IsProductDisplayed(product), Is.True);
        }

        await cartPage.ClickOnContinueShoppingButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Inventory);
        var remainingProducts = ProductTestData.ProductsForCheckout
            .Except(ProductTestData.ProductsForCart)
            .ToArray();
        await inventoryPage.AddProductsToCart(remainingProducts);
        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);
        await cartPage.ClickOnCheckoutButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.CheckoutStepOne);

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillCustomerInformation(
            CheckoutTestData.FirstName,
            CheckoutTestData.LastName,
            CheckoutTestData.PostalCode);
        await checkoutPage.ClickOnContinueButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.CheckoutStepTwo);

        Assert.That(await checkoutPage.GetOverviewTitle(), Is.EqualTo(PageTitleTestData.CheckoutOverview));
        Assert.That(
            await checkoutPage.GetSummaryItemCount(),
            Is.EqualTo(ProductTestData.ProductsForCheckout.Count));
        Assert.That(await checkoutPage.GetSubtotal(), Does.Contain(CheckoutTestData.ItemTotalLabel));
        Assert.That(await checkoutPage.GetTax(), Does.Contain(CheckoutTestData.TaxLabel));
        Assert.That(await checkoutPage.GetTotal(), Does.Contain(CheckoutTestData.TotalLabel));

        foreach (var product in ProductTestData.ProductsForCheckout)
        {
            Assert.That(await checkoutPage.IsProductDisplayedInSummary(product), Is.True);
        }

        await checkoutPage.ClickOnFinishButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.CheckoutComplete);

        Assert.That(await checkoutPage.GetCompleteTitle(), Is.EqualTo(PageTitleTestData.CheckoutComplete));
        Assert.That(
            await checkoutPage.GetConfirmationMessage(),
            Is.EqualTo(CheckoutTestData.OrderConfirmation));
    }

    [TestCase("", CheckoutTestData.LastName, CheckoutTestData.PostalCode, AppErrorTestData.FirstNameRequired)]
    [TestCase(CheckoutTestData.FirstName, "", CheckoutTestData.PostalCode, AppErrorTestData.LastNameRequired)]
    [TestCase(CheckoutTestData.FirstName, CheckoutTestData.LastName, "", AppErrorTestData.PostalCodeRequired)]
    public async Task Checkout_WithMissingRequiredInformation_ShouldShowError(
        string firstName,
        string lastName,
        string postalCode,
        string expectedError)
    {
        var inventoryPage = await LoginFlow.OpenAndLogin(Page, Settings);
        await inventoryPage.AddProductsToCart([ProductTestData.Backpack]);
        await inventoryPage.ClickOnShoppingCartLink();
        await Expect(Page).ToHaveURLAsync(AppRoutes.Cart);

        var cartPage = new CartPage(Page);
        await cartPage.ClickOnCheckoutButton();
        await Expect(Page).ToHaveURLAsync(AppRoutes.CheckoutStepOne);

        var checkoutPage = new CheckoutPage(Page);
        await checkoutPage.FillCustomerInformation(firstName, lastName, postalCode);
        await checkoutPage.ClickOnContinueButton();

        Assert.That(await checkoutPage.IsErrorDisplayed(), Is.True);
        Assert.That(await checkoutPage.GetErrorMessage(), Does.Contain(expectedError));
    }
}
