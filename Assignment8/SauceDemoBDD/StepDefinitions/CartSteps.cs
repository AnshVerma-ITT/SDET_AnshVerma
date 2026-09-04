using NUnit.Framework;
using Reqnroll;
using SauceDemoBDD.Configuration;
using SauceDemoBDD.Support;
using static Microsoft.Playwright.Assertions;

namespace SauceDemoBDD.StepDefinitions;

[Binding]
public sealed class CartSteps
{
    private readonly BrowserDriver _driver;
    private readonly ScenarioState _state;

    public CartSteps(BrowserDriver driver, ScenarioState state)
    {
        _driver = driver;
        _state = state;
    }

    [Then("the cart should contain the selected products")]
    public async Task ThenTheCartShouldContainSelectedProducts()
    {
        var missingProducts = new List<string>();

        foreach (var productName in _state.SelectedProducts)
        {
            if (!await _driver.CartPage.ContainsProduct(productName))
            {
                missingProducts.Add(productName);
            }
        }

        Assert.That(
            missingProducts,
            Is.Empty,
            $"Missing products: {string.Join(", ", missingProducts)}");
    }

    [Then("the cart should contain {int} products")]
    public async Task ThenTheCartShouldContainProducts(int expectedCount)
    {
        Assert.That(await _driver.CartPage.GetProductCount(), Is.EqualTo(expectedCount));
    }

    [When("I continue shopping")]
    public async Task WhenIContinueShopping()
    {
        await _driver.CartPage.ContinueShopping();
        await Expect(_driver.Page).ToHaveURLAsync(AppRoutes.Inventory);
    }

    [When("I start checkout")]
    public async Task WhenIStartCheckout()
    {
        await _driver.CartPage.StartCheckout();
        await Expect(_driver.Page).ToHaveURLAsync(AppRoutes.CheckoutInformation);
    }
}
