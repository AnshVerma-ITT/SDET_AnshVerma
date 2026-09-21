using System.Globalization;
using HRIntimeAutomation.Context;
using HRIntimeAutomation.Utilities;
using HRIntimeAutomation.TestData;
using NUnit.Framework;
using Reqnroll;

namespace HRIntimeAutomation.StepDefinitions;

[Binding]
public sealed class ResignationCalculationSteps
{
    private readonly ScenarioTestContext _context;

    public ResignationCalculationSteps(ScenarioTestContext context) => _context = context;

    [Given("a resignation Date of Apply {string}")]
    public void GivenAResignationDateOfApply(string dateOfApply)
    {
        var parsed = DateTime.TryParseExact(
            dateOfApply,
            ResignationTestData.BoundaryInputFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate);

        Assert.That(parsed, Is.True, $"Could not parse boundary date '{dateOfApply}'.");
        _context.ResignationCalculationInput = parsedDate;
    }

    [When("the configured resignation notice period is calculated")]
    public void WhenTheConfiguredResignationNoticePeriodIsCalculated()
    {
        Assert.That(_context.ResignationCalculationInput, Is.Not.Null);
        _context.CalculatedLastWorkingDate = ResignationDateCalculator.CalculateLastWorkingDate(
            _context.ResignationCalculationInput!.Value,
            ResignationTestData.NoticePeriodMonths,
            ResignationTestData.LastWorkingDayOffset);
    }

    [Then("the calculated Last Working Date should follow the configured calendar-month rule")]
    public void ThenTheCalculatedLastWorkingDateShouldFollowTheConfiguredCalendarMonthRule()
    {
        Assert.That(_context.ResignationCalculationInput, Is.Not.Null);
        Assert.That(_context.CalculatedLastWorkingDate, Is.Not.Null);

        var expected = _context.ResignationCalculationInput!.Value
            .AddMonths(ResignationTestData.NoticePeriodMonths)
            .AddDays(ResignationTestData.LastWorkingDayOffset);
        Assert.That(_context.CalculatedLastWorkingDate, Is.EqualTo(expected));
    }
}
