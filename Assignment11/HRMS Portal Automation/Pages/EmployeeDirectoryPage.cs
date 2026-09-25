using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace HRIntimeAutomation.Pages;

public sealed class EmployeeDirectoryPage : BasePage
{
    private const string JobTitlePlaceholder = "Job Title";
    private const string ViewModePlaceholder = "View Mode";
    private const string PaginationSelector = "div[role='navigation']";
    private const string PaginationButtonSelector = "button";
    private const string CurrentPageSelector = "button[aria-current='page']";
    private const string PageNumberButtonSelector = "button:not([data-dots='true'])";
    private const string EmployeeCardSelector = "div.mantine-Card-root";
    private const string EmployeeCardSectionSelector = "div.mantine-Card-cardSection[data-first='true']";
    private const string TableRowSelector = "tbody tr";
    private const string PageNumberPattern = "^\\d+$";
    private const string ArrowDownKey = "ArrowDown";
    private const string EnterKey = "Enter";

    public EmployeeDirectoryPage(IPage page) : base(page) { }

    public ILocator JobTitleField => Page.GetByPlaceholder(JobTitlePlaceholder, new() { Exact = true });
    public ILocator ViewModeField => Page.GetByPlaceholder(ViewModePlaceholder, new() { Exact = true });
    public ILocator Pagination => Page.Locator(PaginationSelector)
        .Filter(new() { Has = Page.Locator(CurrentPageSelector) });
    public ILocator CurrentPageButton => Pagination.Locator(CurrentPageSelector);
    public ILocator PreviousButton => Pagination.Locator(PaginationButtonSelector).First;
    public ILocator NextButton => Pagination.Locator(PaginationButtonSelector).Last;
    public ILocator EmployeeCards => Page.Locator(EmployeeCardSelector)
        .Filter(new() { Has = Page.Locator(EmployeeCardSectionSelector) });
    public ILocator EmployeeTable => Page.GetByRole(AriaRole.Table);
    public ILocator EmployeeRows => EmployeeTable.Locator(TableRowSelector);

    public async Task ApplyFiltersAsync(string jobTitle, string viewMode)
    {
        await SelectMantineOptionAsync(JobTitleField, jobTitle);
        await SelectMantineOptionAsync(ViewModeField, viewMode);
    }

    public ILocator EmployeeName(string employee) =>
        EmployeeTable.GetByText(employee, new() { Exact = true });

    public ILocator CurrentPage(int pageNumber) =>
        Pagination.Locator($"{CurrentPageSelector}:text-is('{pageNumber}')");

    public Task OpenNextPageAsync() => NextButton.ClickAsync();

    public Task OpenPreviousPageAsync() => PreviousButton.ClickAsync();

    public async Task<IReadOnlyList<string>> ReadVisibleRecordSignaturesAsync() =>
        (await EmployeeCards.AllInnerTextsAsync())
        .Select(text => Regex.Replace(text, @"\s+", " ").Trim())
        .ToArray();

    public async Task<int?> ReadLastPageNumberAsync()
    {
        var lastPageButton = Pagination.Locator(PageNumberButtonSelector)
            .Filter(new() { HasTextRegex = new Regex(PageNumberPattern) })
            .Last;
        var lastPageText = (await lastPageButton.InnerTextAsync()).Trim();
        return int.TryParse(lastPageText, out var lastPage) ? lastPage : null;
    }

    public async Task OpenPageAsync(int pageNumber)
    {
        var pageButton = Pagination.Locator(PageNumberButtonSelector)
            .Filter(new() { HasTextRegex = new Regex($"^{pageNumber}$") })
            .Last;
        await pageButton.ClickAsync();
        await CurrentPage(pageNumber).WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    private async Task SelectMantineOptionAsync(ILocator input, string optionText)
    {
        await input.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await input.ClickAsync();

        var option = Page.GetByRole(AriaRole.Option, new() { Name = optionText, Exact = true });
        if (await option.CountAsync() > 0)
        {
            await option.ClickAsync();
            return;
        }

        await input.FillAsync(optionText);
        await input.PressAsync(ArrowDownKey);
        await input.PressAsync(EnterKey);
    }
}
