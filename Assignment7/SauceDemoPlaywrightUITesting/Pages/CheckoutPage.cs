using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class CheckoutPage
{
    private readonly IPage _page;

    public CheckoutPage(IPage page)
    {
        _page = page;
    }

    public ILocator FirstName => _page.GetByPlaceholder("First Name");
    public ILocator LastName => _page.GetByPlaceholder("Last Name");
    public ILocator PostalCode => _page.GetByPlaceholder("Zip/Postal Code");
    public ILocator ContinueButton => _page.GetByRole(AriaRole.Button, new() { Name = "Continue" });
    public ILocator FinishButton => _page.GetByRole(AriaRole.Button, new() { Name = "Finish" });
    public ILocator ErrorMessage => _page.Locator("[data-test='error']");
    public ILocator OverviewTitle => _page.GetByText("Checkout: Overview");
    public ILocator CompleteTitle => _page.GetByText("Checkout: Complete!");
    public ILocator ConfirmationMessage => _page.GetByText("Thank you for your order!");
    public ILocator SummaryItems => _page.Locator(".cart_item");
    public ILocator Subtotal => _page.Locator(".summary_subtotal_label");
    public ILocator Tax => _page.Locator(".summary_tax_label");
    public ILocator Total => _page.Locator(".summary_total_label");

    public ILocator SummaryItemByProductName(string productName)
    {
        return SummaryItems.Filter(new LocatorFilterOptions { HasText = productName });
    }

    public async Task FillCustomerInformationAsync(string firstName, string lastName, string postalCode)
    {
        await FirstName.FillAsync(firstName);
        await LastName.FillAsync(lastName);
        await PostalCode.FillAsync(postalCode);
    }

    public Task ContinueAsync()
    {
        return ContinueButton.ClickAsync();
    }

    public Task FinishAsync()
    {
        return FinishButton.ClickAsync();
    }
}
