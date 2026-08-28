using Allure.Net.Commons;

namespace SauceDemo.Playwright.Tests.Fixtures;

public static class AllureHelper
{
    public static Task Step(string name, Func<Task> action)
    {
        return AllureApi.Step(name, action);
    }

    public static void AddAttachment(string name, string contentType, string path)
    {
        try
        {
            AllureApi.AddAttachment(name, contentType, path);
        }
        catch
        {
            // NUnit keeps the attachment when Allure is unavailable in the current runner.
        }
    }
}
