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

    private ILocator FirstName => _page.GetByPlaceholder("First Name");
    private ILocator LastName => _page.GetByPlaceholder("Last Name");
    private ILocator PostalCode => _page.GetByPlaceholder("Zip/Postal Code");
    private ILocator ContinueButton => _page.GetByRole(AriaRole.Button, new() { Name = "Continue" });
    private ILocator FinishButton => _page.GetByRole(AriaRole.Button, new() { Name = "Finish" });
    private ILocator ErrorMessage => _page.Locator(ErrorMessageSelector);
    private ILocator OverviewTitle => _page.GetByText("Checkout: Overview");
    private ILocator CompleteTitle => _page.GetByText("Checkout: Complete!");
    private ILocator ConfirmationMessage => _page.GetByText("Thank you for your order!");
    private ILocator SummaryItems => _page.Locator(SummaryItemsSelector);
    private ILocator Subtotal => _page.Locator(SubtotalSelector);
    private ILocator Tax => _page.Locator(TaxSelector);
    private ILocator Total => _page.Locator(TotalSelector);

    public Task<string> GetOverviewTitle()
    {
        return OverviewTitle.InnerTextAsync();
    }

    public Task<string> GetCompleteTitle()
    {
        return CompleteTitle.InnerTextAsync();
    }

    public Task<bool> IsErrorDisplayed()
    {
        return ErrorMessage.IsVisibleAsync();
    }

    public Task<string> GetErrorMessage()
    {
        return ErrorMessage.InnerTextAsync();
    }

    public Task<string> GetConfirmationMessage()
    {
        return ConfirmationMessage.InnerTextAsync();
    }

    public Task<int> GetSummaryItemCount()
    {
        return SummaryItems.CountAsync();
    }

    public Task<string> GetSubtotal()
    {
        return Subtotal.InnerTextAsync();
    }

    public Task<string> GetTax()
    {
        return Tax.InnerTextAsync();
    }

    public Task<string> GetTotal()
    {
        return Total.InnerTextAsync();
    }

    public Task<bool> IsProductDisplayedInSummary(string productName)
    {
        return GetSummaryItemByProductName(productName).IsVisibleAsync();
    }

    public async Task FillCustomerInformation(string firstName, string lastName, string postalCode)
    {
        await FirstName.FillAsync(firstName);
        await LastName.FillAsync(lastName);
        await PostalCode.FillAsync(postalCode);
    }

    public Task ClickOnContinueButton()
    {
        return ContinueButton.ClickAsync();
    }

    public Task ClickOnFinishButton()
    {
        return FinishButton.ClickAsync();
    }

    private ILocator GetSummaryItemByProductName(string productName)
    {
        return SummaryItems.Filter(new LocatorFilterOptions { HasText = productName });
    }
}
