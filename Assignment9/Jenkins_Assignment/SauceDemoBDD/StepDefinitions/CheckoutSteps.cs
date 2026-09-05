using NUnit.Framework;
using Reqnroll;
using SauceDemoBDD.Configuration;
using SauceDemoBDD.Support;
using SauceDemoBDD.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemoBDD.StepDefinitions;

[Binding]
public sealed class CheckoutSteps
{
    private readonly BrowserDriver _driver;
    private readonly ScenarioState _state;

    public CheckoutSteps(BrowserDriver driver, ScenarioState state)
    {
        _driver = driver;
        _state = state;
    }

    [When("I submit valid checkout information")]
    public async Task WhenISubmitValidCheckoutInformation()
    {
        await _driver.CheckoutPage.SubmitCustomerInformation(CheckoutTestData.ValidCustomer);
        await Expect(_driver.Page).ToHaveURLAsync(AppRoutes.CheckoutOverview);
    }

    [When("I submit {string} checkout information")]
    public async Task WhenISubmitInvalidCheckoutInformation(string caseName)
    {
        var checkoutCase = CheckoutTestData.GetCase(caseName);
        _state.ExpectedCheckoutError = checkoutCase.ExpectedError;
        await _driver.CheckoutPage.SubmitCustomerInformation(checkoutCase.Customer);
    }

    [Then("checkout validation should reject the information")]
    public async Task ThenCheckoutShouldRejectTheInformation()
    {
        Assert.That(
            await _driver.CheckoutPage.GetErrorMessage(),
            Does.Contain(_state.ExpectedCheckoutError));
    }

    [Then("the checkout overview should contain {int} products")]
    public async Task ThenTheCheckoutOverviewShouldContainProducts(int expectedCount)
    {
        Assert.That(await _driver.CheckoutPage.GetProductCount(), Is.EqualTo(expectedCount));
    }

    [When("I finish the order")]
    public async Task WhenIFinishTheOrder()
    {
        await _driver.CheckoutPage.FinishOrder();
        await Expect(_driver.Page).ToHaveURLAsync(AppRoutes.CheckoutComplete);
    }

    [Then("the order should be completed successfully")]
    public async Task ThenTheOrderShouldBeCompleted()
    {
        Assert.That(
            await _driver.CheckoutPage.GetConfirmationMessage(),
            Is.EqualTo(CheckoutTestData.ConfirmationMessage));
    }
}
