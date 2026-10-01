using HRIntimeAutomation.Context;
using HRIntimeAutomation.TestData;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class LeaveCorrectionSteps
{
    private readonly ScenarioTestContext _context;

    public LeaveCorrectionSteps(ScenarioTestContext context) => _context = context;

    [When("I apply a two day Work from Home correction with one half day")]
    public async Task WhenIApplyATwoDayWorkFromHomeCorrectionWithOneHalfDay() =>
        _context.LeaveCorrectionResult = await _context.LeaveCorrectionPage.ApplyCorrectionAsync(
            LeaveCorrectionTestData.Request,
            _context.Driver!.Settings.ResponseTimeoutMilliseconds);

    [When("I submit a Leave Correction without {string}")]
    public Task WhenISubmitALeaveCorrectionWithout(string omittedField) =>
        _context.LeaveCorrectionPage.SubmitIncompleteCorrectionAsync(omittedField, LeaveCorrectionTestData.Request);

    [Then("the leave correction success message and created record should be displayed")]
    public async Task ThenTheLeaveCorrectionSuccessMessageAndCreatedRecordShouldBeDisplayed()
    {
        Assert.That(_context.LeaveCorrectionResult, Is.Not.Null,
            "The leave correction result was not saved by the When step.");
        var result = _context.LeaveCorrectionResult!;

        Assert.Multiple(() =>
        {
            Assert.That(result.CorrectionTypeValue, Is.EqualTo(LeaveCorrectionTestData.CorrectionType));
            Assert.That(result.DateRangeValue, Is.Not.Empty);
            Assert.That(result.DescriptionValue, Is.EqualTo(result.Description));
            Assert.That(result.HalfDayTextColor, Is.EqualTo(LeaveCorrectionTestData.HalfDayColor));
            Assert.That(result.HalfDayBorderColor, Is.EqualTo(LeaveCorrectionTestData.HalfDayColor));
            Assert.That(result.NotificationText,
                Does.Contain(LeaveCorrectionTestData.SuccessMessage).IgnoreCase);
        });

        var createdRecord = _context.LeaveCorrectionPage.CreatedRecord(
            result.ExpectedDateRange,
            LeaveCorrectionTestData.CorrectionType);
        await Assertions.Expect(createdRecord).ToBeVisibleAsync();
        await Assertions.Expect(createdRecord).ToContainTextAsync(result.ExpectedDateRange);
        await Assertions.Expect(createdRecord).ToContainTextAsync(LeaveCorrectionTestData.CorrectionType);
        await Assertions.Expect(createdRecord).ToContainTextAsync(LeaveCorrectionTestData.ExpectedDuration);

        var cells = createdRecord.GetByRole(AriaRole.Cell);
        Assert.That(await cells.CountAsync(),
            Is.GreaterThanOrEqualTo(LeaveCorrectionTestData.MinimumExpectedCellCount),
            "The created Leave Correction row does not contain the expected status cell.");
        var status = (await cells.Nth(LeaveCorrectionTestData.StatusCellIndex).InnerTextAsync()).Trim();
        Assert.That(status, Is.Not.Empty,
            "The created Leave Correction record has an empty status.");
    }

    [Then("the incomplete Leave Correction should be rejected")]
    public void ThenTheIncompleteLeaveCorrectionShouldBeRejected()
    {
        var page = _context.LeaveCorrectionPage;
        var hasValidationEvidence = !page.LastSubmitButtonEnabled
            || page.LastValidationMessages.Count > 0
            || !string.IsNullOrWhiteSpace(page.LastValidationNotification);

        Assert.Multiple(() =>
        {
            Assert.That(page.LastDialogVisible, Is.True,
                "The incomplete Leave Correction dialog closed as if submission succeeded.");
            Assert.That(page.LastValidationNotification ?? string.Empty,
                Does.Not.Contain(LeaveCorrectionTestData.SuccessMessage).IgnoreCase);
            Assert.That(hasValidationEvidence, Is.True,
                $"No validation was exposed when '{page.LastOmittedField}' was omitted.");
        });
    }
}
