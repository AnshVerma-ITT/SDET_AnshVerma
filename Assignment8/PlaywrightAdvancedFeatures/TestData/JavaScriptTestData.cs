namespace PlaywrightAdvancedFeatures.TestData;

public static class JavaScriptTestData
{
    public const int FirstCaseFirstValue = 2;
    public const int FirstCaseSecondValue = 3;
    public const int FirstCaseThirdValue = 5;
    public const int FirstCaseExpectedTotal = 10;

    public const int SecondCaseFirstValue = 10;
    public const int SecondCaseSecondValue = 20;
    public const int SecondCaseThirdValue = 30;
    public const int SecondCaseExpectedTotal = 60;

    public const string SumScript = "values => values.reduce((total, value) => total + value, 0)";
}
