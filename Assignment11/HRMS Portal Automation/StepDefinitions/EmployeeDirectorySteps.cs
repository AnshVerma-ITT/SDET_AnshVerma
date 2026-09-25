using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class EmployeeDirectorySteps
{
    private const string ActivePageAttribute = "data-active";
    private const string AriaDisabledAttribute = "aria-disabled";
    private const string TrueAttributeValue = "true";

    private readonly ScenarioTestContext _context;

    public EmployeeDirectorySteps(ScenarioTestContext context) => _context = context;

    [When("I navigate to Employee Directory")]
    public async Task WhenINavigateToEmployeeDirectory()
    {
        await _context.NavigationPage.OpenOrganizationAsync();
        await _context.NavigationPage.OpenEmployeeDirectoryAsync();
    }

    [When("I filter Job Title as Director of Engineering and select Table View")]
    public Task WhenIFilterJobTitleAndSelectTableView() =>
        _context.EmployeeDirectoryPage.ApplyFiltersAsync(
            EmployeeDirectoryTestData.JobTitle,
            EmployeeDirectoryTestData.ViewMode);

    [Then("every returned employee should match the selected job title")]
    public async Task ThenEveryEmployeeShouldMatchTheSelectedJobTitle()
    {
        await Assertions.Expect(_context.EmployeeDirectoryPage.JobTitleField)
            .ToHaveValueAsync(EmployeeDirectoryTestData.JobTitle);
        await Assertions.Expect(_context.EmployeeDirectoryPage.ViewModeField)
            .ToHaveValueAsync(EmployeeDirectoryTestData.ViewMode);
        await Assertions.Expect(_context.EmployeeDirectoryPage.EmployeeTable).ToBeVisibleAsync();

        var rowCount = await _context.EmployeeDirectoryPage.EmployeeRows.CountAsync();
        Assert.That(rowCount, Is.GreaterThan(0), "The selected job-title filter returned no employees.");
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            await Assertions.Expect(_context.EmployeeDirectoryPage.EmployeeRows.Nth(rowIndex))
                .ToContainTextAsync(EmployeeDirectoryTestData.JobTitle);
        }
    }

    [Then("Employee Directory pagination should show no more than 12 records and have correct navigation states")]
    public async Task ThenEmployeeDirectoryPaginationShouldBeValid()
    {
        var page = _context.EmployeeDirectoryPage;
        await Assertions.Expect(page.Pagination).ToBeVisibleAsync();

        await AssertCurrentPageAsync(EmployeeDirectoryTestData.FirstPage);
        await AssertRecordCountAsync();
        var firstPageRecords = await page.ReadVisibleRecordSignaturesAsync();
        await AssertButtonStateAsync(page.PreviousButton, shouldBeEnabled: false);
        await AssertButtonStateAsync(page.NextButton, shouldBeEnabled: true);

        await page.OpenNextPageAsync();
        await Assertions.Expect(page.CurrentPage(EmployeeDirectoryTestData.SecondPage)).ToBeVisibleAsync();
        await AssertCurrentPageAsync(EmployeeDirectoryTestData.SecondPage);
        await AssertRecordCountAsync();
        var secondPageRecords = await page.ReadVisibleRecordSignaturesAsync();
        Assert.That(string.Join("|", secondPageRecords),
            Is.Not.EqualTo(string.Join("|", firstPageRecords)),
            "Employee Directory records did not change after moving to the next page.");
        await AssertButtonStateAsync(page.PreviousButton, shouldBeEnabled: true);
        await AssertButtonStateAsync(page.NextButton, shouldBeEnabled: true);

        await page.OpenPreviousPageAsync();
        await Assertions.Expect(page.CurrentPage(EmployeeDirectoryTestData.FirstPage)).ToBeVisibleAsync();
        await AssertCurrentPageAsync(EmployeeDirectoryTestData.FirstPage);
        var returnedFirstPageRecords = await page.ReadVisibleRecordSignaturesAsync();
        Assert.That(returnedFirstPageRecords, Is.EqualTo(firstPageRecords),
            "Returning to page 1 did not restore the original Employee Directory records.");

        await page.OpenNextPageAsync();
        await Assertions.Expect(page.CurrentPage(EmployeeDirectoryTestData.SecondPage)).ToBeVisibleAsync();

        var lastPage = await page.ReadLastPageNumberAsync();
        Assert.That(lastPage, Is.Not.Null.And.GreaterThanOrEqualTo(EmployeeDirectoryTestData.SecondPage),
            "The last Employee Directory page number could not be determined.");

        await page.OpenPageAsync(lastPage!.Value);
        await AssertCurrentPageAsync(lastPage.Value);
        await AssertRecordCountAsync();
        await AssertButtonStateAsync(page.PreviousButton, shouldBeEnabled: true);
        await AssertButtonStateAsync(page.NextButton, shouldBeEnabled: false);
    }

    private async Task AssertRecordCountAsync()
    {
        await _context.EmployeeDirectoryPage.EmployeeCards.First
            .WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var count = await _context.EmployeeDirectoryPage.EmployeeCards.CountAsync();
        Assert.That(count, Is.InRange(1, EmployeeDirectoryTestData.MaximumRecordsPerPage),
            "Employee Directory record count is outside the expected page-size range.");
    }

    private async Task AssertCurrentPageAsync(int expectedPage)
    {
        var currentPage = _context.EmployeeDirectoryPage.CurrentPageButton;
        await Assertions.Expect(currentPage).ToHaveCountAsync(1);
        await Assertions.Expect(currentPage).ToHaveTextAsync(expectedPage.ToString());
        await Assertions.Expect(currentPage).ToHaveAttributeAsync(ActivePageAttribute, TrueAttributeValue);
    }

    private static async Task AssertButtonStateAsync(
        ILocator button,
        bool shouldBeEnabled)
    {
        await Assertions.Expect(button).ToBeVisibleAsync();
        if (shouldBeEnabled)
        {
            await Assertions.Expect(button).ToBeEnabledAsync();
            return;
        }

        await Assertions.Expect(button).ToBeDisabledAsync();
        await Assertions.Expect(button).ToHaveAttributeAsync(AriaDisabledAttribute, TrueAttributeValue);
    }
}
