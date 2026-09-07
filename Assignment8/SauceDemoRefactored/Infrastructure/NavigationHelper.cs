using Microsoft.Playwright;
using SauceDemo.Playwright.Tests.Configuration;

namespace SauceDemo.Playwright.Tests.Infrastructure;

public static class NavigationHelper
{
    private static readonly string[] RetryableNetworkErrors =
    [
        "ERR_NETWORK_CHANGED",
        "ERR_CONNECTION_RESET",
        "ERR_CONNECTION_CLOSED",
        "ERR_INTERNET_DISCONNECTED",
        "ERR_NAME_NOT_RESOLVED",
        "ERR_TIMED_OUT",
        "The network connection was lost",
        "NSURLErrorDomain"
    ];

    public static async Task NavigateTo(IPage page, string url, TestSettings settings)
    {
        for (var attempt = 1; attempt <= settings.NavigationMaxAttempts; attempt++)
        {
            try
            {
                await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.Commit
                });
                return;
            }
            catch (TimeoutException) when (attempt < settings.NavigationMaxAttempts)
            {
                await Task.Delay(settings.NavigationRetryDelayMilliseconds);
            }
            catch (PlaywrightException exception) when (
                attempt < settings.NavigationMaxAttempts && IsRetryableNetworkError(exception.Message))
            {
                await Task.Delay(settings.NavigationRetryDelayMilliseconds);
            }
        }
    }

    private static bool IsRetryableNetworkError(string message)
    {
        return RetryableNetworkErrors.Any(error =>
            message.Contains(error, StringComparison.OrdinalIgnoreCase));
    }
}
