using System.Globalization;
using System.Text.RegularExpressions;
using HRIntimeAutomation.Configuration;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using HRIntimeAutomation.Utilities;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class ResignationSteps
{
    private const string AriaSelectedAttribute = "aria-selected";
    private const string TrueAttributeValue = "true";
    private readonly ScenarioTestContext _context;

    public ResignationSteps(ScenarioTestContext context) => _context = context;

    [When("I open Employment Resignation")]
    public Task OpenEmploymentResignation() =>
        _context.MyProfilePage.OpenEmploymentResignationAsync();

    [Then("the Last Working Date should follow the two month resignation calculation")]
    public async Task LastWorkingDateFollowsTwoMonthCalculation()
    {
        var page = _context.MyProfilePage;
        await page.Pages.AssertUrlAsync(AppRoutes.MyProfile);
        await Assertions.Expect(page.EmploymentTab)
            .ToHaveAttributeAsync(AriaSelectedAttribute, TrueAttributeValue);
        await Assertions.Expect(page.ResignationTab)
            .ToHaveAttributeAsync(AriaSelectedAttribute, TrueAttributeValue);
        await Assertions.Expect(page.ResignationPanel).ToBeVisibleAsync();
        await Assertions.Expect(page.DateOfApplyInput).ToHaveCountAsync(1);
        await Assertions.Expect(page.LastWorkingDateInput).ToHaveCountAsync(1);

        var datePattern = new Regex(ResignationTestData.DateValuePattern);
        await Assertions.Expect(page.DateOfApplyInput).ToHaveValueAsync(datePattern);
        await Assertions.Expect(page.LastWorkingDateInput).ToHaveValueAsync(datePattern);

        var dateOfApplyText = (await page.DateOfApplyInput.InputValueAsync()).Trim();
        var lastWorkingDateText = (await page.LastWorkingDateInput.InputValueAsync()).Trim();
        var dateOfApplyParsed = DateTime.TryParseExact(
            dateOfApplyText,
            ResignationTestData.DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var dateOfApply);
        var lastWorkingDateParsed = DateTime.TryParseExact(
            lastWorkingDateText,
            ResignationTestData.DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var actualLastWorkingDate);

        Assert.That(dateOfApplyParsed, Is.True,
            $"Date of Apply '{dateOfApplyText}' has an unexpected format.");
        Assert.That(lastWorkingDateParsed, Is.True,
            $"Last Working Date '{lastWorkingDateText}' has an unexpected format.");

        var expectedLastWorkingDate = ResignationDateCalculator.CalculateLastWorkingDate(
            dateOfApply,
            ResignationTestData.NoticePeriodMonths,
            ResignationTestData.LastWorkingDayOffset);
        Assert.That(actualLastWorkingDate, Is.EqualTo(expectedLastWorkingDate),
            $"Last Working Date should be {expectedLastWorkingDate.ToString(ResignationTestData.DateFormat, CultureInfo.InvariantCulture)}.");
    }
}
