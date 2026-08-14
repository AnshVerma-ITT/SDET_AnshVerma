using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightApiFramework.Framework.Reporting;

public static class ReportHelper
{
    public static void PrintTest(string testName)
    {
        TestContext.Progress.WriteLine("");
        TestContext.Progress.WriteLine("========== " + testName + " ==========");
    }

    public static void PrintStep(string step)
    {
        TestContext.Progress.WriteLine("STEP: " + step);
    }

    public static void PrintResponse(string testName, IAPIResponse response)
    {
        TestContext.Progress.WriteLine(testName + " -> " + response.Status + " " + response.StatusText);
    }

    public static void PrintValue(string name, object value)
    {
        TestContext.Progress.WriteLine(name + ": " + value);
    }
}
