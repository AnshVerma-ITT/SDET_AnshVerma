using NUnit.Framework;
using PlaywrightAdvancedFeatures.Configuration;
using PlaywrightAdvancedFeatures.Fixtures;
using PlaywrightAdvancedFeatures.TestData;

namespace PlaywrightAdvancedFeatures.Tests;

[Category(TestCategories.AdvancedFeatures)]
public sealed class JavaScriptEvaluationTests : BrowserTestBase
{
    [TestCase(
        JavaScriptTestData.FirstCaseFirstValue,
        JavaScriptTestData.FirstCaseSecondValue,
        JavaScriptTestData.FirstCaseThirdValue,
        JavaScriptTestData.FirstCaseExpectedTotal)]
    [TestCase(
        JavaScriptTestData.SecondCaseFirstValue,
        JavaScriptTestData.SecondCaseSecondValue,
        JavaScriptTestData.SecondCaseThirdValue,
        JavaScriptTestData.SecondCaseExpectedTotal)]
    public async Task JavaScript_EvaluateParameterizedValues_ShouldReturnTotal(
        int first,
        int second,
        int third,
        int expectedTotal)
    {
        var actualTotal = await Page.EvaluateAsync<int>(
            JavaScriptTestData.SumScript,
            new[] { first, second, third });

        Assert.That(actualTotal, Is.EqualTo(expectedTotal));
    }
}
