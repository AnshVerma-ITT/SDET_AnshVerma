using Microsoft.Playwright;

namespace HRIntimeAutomation.Wrappers;

public sealed class PageActions
{
    private readonly IPage _page;
    private readonly Uri _baseUri;

    public PageActions(IPage page, string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(page);
        _page = page;
        _baseUri = Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            ? uri
            : throw new ArgumentException("Base URL must be absolute.", nameof(baseUrl));
    }

    public async Task OpenAsync(string route)
    {
        await _page.GotoAsync(route, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await AssertUrlAsync(route);
    }

    public async Task ClickAndAssertUrlAsync(ILocator locator, string route)
    {
        await locator.ClickAsync();
        await AssertUrlAsync(route);
    }

    public async Task AssertUrlAsync(string route)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        await _page.WaitForURLAsync(url => RouteMatches(url, route));
        if (!RouteMatches(_page.Url, route))
            throw new PlaywrightException($"Expected route '{route}', but opened '{_page.Url}'.");
    }

    public Task ReloadAsync() => _page.ReloadAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
    public Task GoBackAsync() => _page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });

    public bool RouteMatches(string actualUrl, string expectedRoute)
    {
        if (!Uri.TryCreate(actualUrl, UriKind.Absolute, out var actual))
            return false;

        var expected = new Uri(_baseUri, expectedRoute);
        return SameOrigin(actual, expected)
            && NormalizePath(actual.AbsolutePath) == NormalizePath(expected.AbsolutePath)
            && QueriesMatch(actual.Query, expected.Query);
    }

    private static bool SameOrigin(Uri actual, Uri expected) =>
        string.Equals(actual.Scheme, expected.Scheme, StringComparison.OrdinalIgnoreCase)
        && string.Equals(actual.Host, expected.Host, StringComparison.OrdinalIgnoreCase)
        && actual.Port == expected.Port;

    private static string NormalizePath(string path) => path.Length > 1 ? path.TrimEnd('/') : path;

    private static bool QueriesMatch(string actual, string expected) =>
        ParseQuery(actual).OrderBy(pair => pair.Key).ThenBy(pair => pair.Value)
            .SequenceEqual(ParseQuery(expected).OrderBy(pair => pair.Key).ThenBy(pair => pair.Value));

    private static IEnumerable<KeyValuePair<string, string>> ParseQuery(string query)
    {
        foreach (var part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var values = part.Split('=', 2);
            yield return new(
                Uri.UnescapeDataString(values[0].Replace('+', ' ')),
                Uri.UnescapeDataString((values.Length == 2 ? values[1] : string.Empty).Replace('+', ' ')));
        }
    }
}
