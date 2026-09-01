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
        foreach (var productName in _state.SelectedProducts)
        {
            Assert.That(await _driver.CartPage.ContainsProduct(productName), Is.True);
        }
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
