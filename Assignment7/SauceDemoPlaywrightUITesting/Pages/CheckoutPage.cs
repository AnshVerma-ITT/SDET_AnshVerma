using Microsoft.Playwright;

namespace SauceDemo.Playwright.Tests.Pages;

public sealed class CheckoutPage
{
    private const string ErrorMessageSelector = "[data-test='error']";
    private const string SummaryItemsSelector = ".cart_item";
    private const string SubtotalSelector = ".summary_subtotal_label";
    private const string TaxSelector = ".summary_tax_label";
    private const string TotalSelector = ".summary_total_label";

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
    public ILocator ErrorMessage => _page.Locator(ErrorMessageSelector);
    public ILocator OverviewTitle => _page.GetByText("Checkout: Overview");
    public ILocator CompleteTitle => _page.GetByText("Checkout: Complete!");
    public ILocator ConfirmationMessage => _page.GetByText("Thank you for your order!");
    public ILocator SummaryItems => _page.Locator(SummaryItemsSelector);
    public ILocator Subtotal => _page.Locator(SubtotalSelector);
    public ILocator Tax => _page.Locator(TaxSelector);
    public ILocator Total => _page.Locator(TotalSelector);

    public ILocator SummaryItemByProductName(string productName)
    {
        return SummaryItems.Filter(new LocatorFilterOptions { HasText = productName });
    }

    public async Task FillCustomerInformation(string firstName, string lastName, string postalCode)
    {
        await FirstName.FillAsync(firstName);
        await LastName.FillAsync(lastName);
        await PostalCode.FillAsync(postalCode);
    }

    public Task Continue()
    {
        return ContinueButton.ClickAsync();
    }

    public Task Finish()
    {
        return FinishButton.ClickAsync();
    }
}
