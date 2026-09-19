using System.Globalization;
using System.Text.RegularExpressions;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class MyProfileSteps
{
    private const string AriaSelectedAttribute = "aria-selected";
    private const string AriaExpandedAttribute = "aria-expanded";
    private const string AriaHiddenAttribute = "aria-hidden";
    private const string TrueAttributeValue = "true";
    private const string FalseAttributeValue = "false";
    private const string WhitespacePattern = @"\s+";

    private readonly ScenarioTestContext _context;

    public MyProfileSteps(ScenarioTestContext context) => _context = context;

    [When("I navigate to Organization My Profile")]
    public Task WhenINavigateToOrganizationMyProfile() =>
        _context.MyProfilePage.NavigateToMyProfileAsync();

    [Then("the top personal information should match the expected employee data")]
    public async Task ThenTheTopPersonalInformationShouldMatch()
    {
        await Assertions.Expect(_context.MyProfilePage.EmployeeName(ProfileTestData.EmployeeName))
            .ToBeVisibleAsync();

        foreach (var expectedField in ProfileTestData.PersonalInformation)
        {
            var fieldRow = _context.MyProfilePage.TopFieldRow(expectedField.Label);
            await Assertions.Expect(fieldRow).ToBeVisibleAsync();
            await Assertions.Expect(fieldRow).ToContainTextAsync(expectedField.Value);
        }
    }

    [When("I open Job and Skills")]
    public Task WhenIOpenJobAndSkills() => _context.MyProfilePage.OpenJobAndSkillsAsync();

    [Then("the Job section should match the expected job data")]
    public async Task ThenTheJobSectionShouldMatch()
    {
        var page = _context.MyProfilePage;
        await Assertions.Expect(page.JobAndSkillsTab)
            .ToHaveAttributeAsync(AriaSelectedAttribute, TrueAttributeValue);

        var jobPanel = await page.ExpandJobSectionAsync();
        Assert.That(jobPanel, Is.Not.Null,
            "Job accordion does not expose an aria-controls panel id.");
        await Assertions.Expect(page.JobAccordionButton)
            .ToHaveAttributeAsync(AriaExpandedAttribute, TrueAttributeValue);
        await Assertions.Expect(jobPanel!).ToHaveAttributeAsync(AriaHiddenAttribute, FalseAttributeValue);
        await Assertions.Expect(jobPanel!).ToBeVisibleAsync();

        foreach (var expectedField in ProfileTestData.JobInformation)
        {
            await Assertions.Expect(jobPanel!).ToContainTextAsync(expectedField.Label);
            await Assertions.Expect(jobPanel!).ToContainTextAsync(expectedField.Value);
        }
    }

    [Then("the current work scheme should show Saturday and Sunday as week off")]
    public async Task ThenTheCurrentWorkSchemeShouldShowWeekendAsWeekOff()
    {
        var page = _context.MyProfilePage;
        var workSchemePanel = await page.ExpandWorkSchemeAsync();
        Assert.That(workSchemePanel, Is.Not.Null,
            "Work Scheme Record accordion does not expose an aria-controls panel id.");

        await Assertions.Expect(page.WorkSchemeAccordionButton)
            .ToHaveAttributeAsync(AriaExpandedAttribute, TrueAttributeValue);
        await Assertions.Expect(workSchemePanel!).ToBeVisibleAsync();

        var schemeName = page.SchemeName(workSchemePanel!, ProfileTestData.CurrentSchemeName);
        await Assertions.Expect(schemeName).ToHaveCountAsync(1);

        var currentScheme = page.CurrentScheme(workSchemePanel!, ProfileTestData.CurrentSchemeName);
        await Assertions.Expect(currentScheme).ToBeVisibleAsync();
        await Assertions.Expect(currentScheme).ToContainTextAsync(ProfileTestData.CurrentSchemeStatus);
        await Assertions.Expect(page.SchemeDetailsButton(currentScheme)).ToHaveCountAsync(1);

        await page.OpenSchemeDetailsAsync(currentScheme);
        await Assertions.Expect(page.SchemeDialog).ToBeVisibleAsync();
        var dialogText = Regex.Replace(await page.SchemeDialog.InnerTextAsync(), WhitespacePattern, " ").Trim();

        Assert.Multiple(() =>
        {
            Assert.That(dialogText, Does.Match(ProfileTestData.SaturdayWeekOffPattern));
            Assert.That(dialogText, Does.Match(ProfileTestData.SundayWeekOffPattern));
        });
    }

    [When("I open Employment Resignation")]
    public Task WhenIOpenEmploymentResignation() =>
        _context.MyProfilePage.OpenEmploymentResignationAsync();

    [Then("the Last Working Date should follow the two month resignation calculation")]
    public async Task ThenTheLastWorkingDateShouldFollowTwoMonthCalculation()
    {
        var page = _context.MyProfilePage;
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

        var expectedLastWorkingDate = dateOfApply
            .AddMonths(ResignationTestData.NoticePeriodMonths)
            .AddDays(ResignationTestData.LastWorkingDayOffset);
        Assert.That(actualLastWorkingDate, Is.EqualTo(expectedLastWorkingDate),
            $"Last Working Date should be {expectedLastWorkingDate.ToString(ResignationTestData.DateFormat, CultureInfo.InvariantCulture)}.");
    }
}
