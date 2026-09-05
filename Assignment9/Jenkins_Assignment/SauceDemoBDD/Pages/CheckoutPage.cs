using Microsoft.Playwright;
using SauceDemoBDD.TestData;

namespace SauceDemoBDD.Pages;

public sealed class CheckoutPage
{
    private const string FirstNameTestId = "firstName";
    private const string LastNameTestId = "lastName";
    private const string PostalCodeTestId = "postalCode";
    private const string ContinueTestId = "continue";
    private const string FinishTestId = "finish";
    private const string ErrorTestId = "error";
    private const string SummaryItemsSelector = ".cart_item";
    private const string ConfirmationTestId = "complete-header";

    private readonly IPage _page;

    public CheckoutPage(IPage page)
    {
        _page = page;
    }

    private ILocator FirstName => _page.GetByTestId(FirstNameTestId);
    private ILocator LastName => _page.GetByTestId(LastNameTestId);
    private ILocator PostalCode => _page.GetByTestId(PostalCodeTestId);
    private ILocator ContinueButton => _page.GetByTestId(ContinueTestId);
    private ILocator FinishButton => _page.GetByTestId(FinishTestId);
    private ILocator ErrorMessage => _page.GetByTestId(ErrorTestId);
    private ILocator SummaryItems => _page.Locator(SummaryItemsSelector);
    private ILocator Confirmation => _page.GetByTestId(ConfirmationTestId);

    public async Task SubmitCustomerInformation(Customer customer)
    {
        await FirstName.FillAsync(customer.FirstName);
        await LastName.FillAsync(customer.LastName);
        await PostalCode.FillAsync(customer.PostalCode);
        await ContinueButton.ClickAsync();
    }

    public Task<string> GetErrorMessage()
    {
        return ErrorMessage.InnerTextAsync();
    }

    public Task<int> GetProductCount()
    {
        return SummaryItems.CountAsync();
    }

    public Task FinishOrder()
    {
        return FinishButton.ClickAsync();
    }

    public Task<string> GetConfirmationMessage()
    {
        return Confirmation.InnerTextAsync();
    }
}
