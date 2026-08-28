using NUnit.Framework;
using Reqnroll;
using SauceDemoBDD.Configuration;
using SauceDemoBDD.Support;
using SauceDemoBDD.TestData;
using static Microsoft.Playwright.Assertions;

namespace SauceDemoBDD.StepDefinitions;

[Binding]
public sealed class InventorySteps
{
    private readonly BrowserDriver _driver;
    private readonly ScenarioState _state;

    public InventorySteps(BrowserDriver driver, ScenarioState state)
    {
        _driver = driver;
        _state = state;
    }

    [When("I sort products by price from low to high")]
    public Task WhenISortProductsByPrice()
    {
        return _driver.InventoryPage.SortByPriceLowToHigh();
    }

    [Then("product prices should be in ascending order")]
    public async Task ThenPricesShouldBeAscending()
    {
        var actualPrices = await _driver.InventoryPage.GetProductPrices();
        Assert.That(actualPrices, Is.EqualTo(actualPrices.OrderBy(price => price)));
    }

    [When("I add these products to the cart")]
    public async Task WhenIAddTheseProducts(Table table)
    {
        var productNames = table.Rows
            .Select(row => ProductCatalog.GetName(row["Product"]))
            .ToArray();
        _state.SelectedProducts.AddRange(productNames);
        await _driver.InventoryPage.AddProducts(productNames);
    }

    [When("I add {string} to the cart")]
    public async Task WhenIAddAProduct(string productKey)
    {
        var productName = ProductCatalog.GetName(productKey);
        _state.SelectedProducts.Add(productName);
        await _driver.InventoryPage.AddProducts([productName]);
    }

    [When("I remove {string} from the inventory")]
    public async Task WhenIRemoveAProduct(string productKey)
    {
        var productName = ProductCatalog.GetName(productKey);
        await _driver.InventoryPage.RemoveProduct(productName);
        _state.SelectedProducts.Remove(productName);
    }

    [Then("the cart badge should show {int} item(s)")]
    public async Task ThenTheCartBadgeShouldShow(int expectedCount)
    {
        Assert.That(await _driver.InventoryPage.GetCartCount(), Is.EqualTo(expectedCount));
    }

    [When("I open the shopping cart")]
    public async Task WhenIOpenTheShoppingCart()
    {
        await _driver.InventoryPage.OpenCart();
        await Expect(_driver.Page).ToHaveURLAsync(AppRoutes.Cart);
    }
}
