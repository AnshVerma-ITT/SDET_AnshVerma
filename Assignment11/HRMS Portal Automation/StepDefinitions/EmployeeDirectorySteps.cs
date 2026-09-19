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

    [Then("Archit Jain, Kapil Paliwal, and Yatin Yogi should be present")]
    public async Task ThenTheThreeDirectorsShouldBePresent()
    {
        await Assertions.Expect(_context.EmployeeDirectoryPage.JobTitleField)
            .ToHaveValueAsync(EmployeeDirectoryTestData.JobTitle);
        await Assertions.Expect(_context.EmployeeDirectoryPage.ViewModeField)
            .ToHaveValueAsync(EmployeeDirectoryTestData.ViewMode);
        await Assertions.Expect(_context.EmployeeDirectoryPage.EmployeeTable).ToBeVisibleAsync();

        foreach (var employee in EmployeeDirectoryTestData.ExpectedEmployees)
        {
            var employeeName = _context.EmployeeDirectoryPage.EmployeeName(employee);
            await Assertions.Expect(employeeName).ToHaveCountAsync(1);
            await Assertions.Expect(employeeName).ToBeVisibleAsync();
        }
    }

    [Then("Employee Directory pagination should show no more than 12 records and have correct navigation states")]
    public async Task ThenEmployeeDirectoryPaginationShouldBeValid()
    {
        var page = _context.EmployeeDirectoryPage;
        await Assertions.Expect(page.Pagination).ToBeVisibleAsync();

        await AssertCurrentPageAsync(EmployeeDirectoryTestData.FirstPage);
        await AssertRecordCountAsync();
        await AssertButtonStateAsync(page.PreviousButton, shouldBeEnabled: false);
        await AssertButtonStateAsync(page.NextButton, shouldBeEnabled: true);

        await page.OpenNextPageAsync();
        await Assertions.Expect(page.CurrentPage(EmployeeDirectoryTestData.SecondPage)).ToBeVisibleAsync();
        await AssertCurrentPageAsync(EmployeeDirectoryTestData.SecondPage);
        await AssertRecordCountAsync();
        await AssertButtonStateAsync(page.PreviousButton, shouldBeEnabled: true);
        await AssertButtonStateAsync(page.NextButton, shouldBeEnabled: true);

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
