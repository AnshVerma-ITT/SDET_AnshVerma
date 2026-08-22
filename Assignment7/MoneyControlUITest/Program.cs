using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

var result = await Nifty50PriceReader.GetPriceFiveMinutesBeforeLatestAsync();

Console.WriteLine("Moneycontrol NIFTY 50 data retrieved from browser-inspected chart data");
Console.WriteLine($"Opened page          : {result.PageUrl}");
Console.WriteLine($"Chart data source    : {result.ChartSourceUrl}");
Console.WriteLine($"Latest chart time    : {result.Latest.Time:yyyy-MM-dd HH:mm}");
Console.WriteLine($"Latest chart price   : {result.Latest.Price:N2}");
Console.WriteLine($"Requested chart time : {result.TargetTime:yyyy-MM-dd HH:mm}");
Console.WriteLine($"Matched chart time   : {result.Match.Time:yyyy-MM-dd HH:mm}{(result.IsExactMatch ? string.Empty : " (nearest earlier available tick)")}");
Console.WriteLine($"NIFTY 50 price       : {result.Match.Price:N2}");

internal static class Nifty50PriceReader
{
    private const string MarketActionPageUrl = "https://www.moneycontrol.com/stocksmarketsindia/";
    private const string NiftyFrameSelector = "iframe#nif_load_graph";
    private const int FrameAttachTimeoutMs = 15_000;
    private const string NiftyFallbackChartUrl =
        "https://www.moneycontrol.com/mccode/common/indices_chart/indices_chart.php?classic=true&market=i&period=1d&ind_id=9&width=100%25&height=200";

    private static readonly Regex ChartPointRegex = new(
        @"Date\.UTC\((?<year>\d{4}),(?<month>\d{1,2}),(?<day>\d{1,2}),(?<hour>\d{1,2}),(?<minute>\d{1,2})\)\s*,\s*(?<price>\d+(?:\.\d+)?)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static async Task<NiftyResult> GetPriceFiveMinutesBeforeLatestAsync()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "en-IN",
            TimezoneId = "Asia/Kolkata"
        });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(60_000);

        try
        {
            await page.GotoAsync(MarketActionPageUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60_000
            });
        }
        catch (TimeoutException)
        {
            // The chart source fallback below still retrieves the Market Action data when the ad-heavy page stalls.
        }

        var chartSource = await GetNiftyChartHtmlAsync(page, context);
        var ticks = ParseChartTicks(chartSource.Html);

        if (ticks.Count == 0)
        {
            var snippet = Regex.Replace(chartSource.Html, @"\s+", " ");
            snippet = snippet.Length > 500 ? snippet[..500] : snippet;
            throw new InvalidOperationException(
                $"No NIFTY 50 chart points were found in the Moneycontrol chart source. Source: {chartSource.Url}. First HTML: {snippet}");
        }

        var orderedTicks = ticks.OrderBy(tick => tick.Time).ToList();
        var latest = orderedTicks[^1];
        var targetTime = latest.Time.AddMinutes(-5);
        var exactMatch = orderedTicks.FirstOrDefault(tick => tick.Time == targetTime);
        var isExactMatch = exactMatch is not null;
        var match = exactMatch
            ?? orderedTicks.LastOrDefault(tick => tick.Time <= targetTime)
            ?? orderedTicks[0];

        return new NiftyResult(
            MarketActionPageUrl,
            chartSource.Url,
            latest,
            targetTime,
            match,
            isExactMatch);
    }

    private static async Task<ChartSource> GetNiftyChartHtmlAsync(IPage page, IBrowserContext context)
    {
        try
        {
            await page.Locator(NiftyFrameSelector).WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = FrameAttachTimeoutMs
            });
        }
        catch (TimeoutException)
        {
            return await OpenChartSourceAsync(NiftyFallbackChartUrl);
        }

        var frame = page.Frames.FirstOrDefault(IsNiftyChartFrame);
        if (frame is not null)
        {
            return new ChartSource(await frame.ContentAsync(), frame.Url);
        }

        var iframeSource = await page.Locator(NiftyFrameSelector).GetAttributeAsync("src");
        var chartUrl = string.IsNullOrWhiteSpace(iframeSource)
            ? NiftyFallbackChartUrl
            : ToAbsoluteUrl(iframeSource, page.Url);

        return await OpenChartSourceAsync(chartUrl);
    }

    private static async Task<ChartSource> OpenChartSourceAsync(string chartUrl)
    {
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Referrer = new Uri(MarketActionPageUrl);
        httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

        var html = await httpClient.GetStringAsync(chartUrl);
        return new ChartSource(html, chartUrl);
    }

    private static bool IsNiftyChartFrame(IFrame frame)
    {
        return frame.Url.Contains("indices_chart.php", StringComparison.OrdinalIgnoreCase)
            && frame.Url.Contains("ind_id=9", StringComparison.OrdinalIgnoreCase);
    }

    private static string ToAbsoluteUrl(string url, string baseUrl)
    {
        var decodedUrl = WebUtility.HtmlDecode(url);
        if (decodedUrl.StartsWith("//", StringComparison.Ordinal))
        {
            return $"https:{decodedUrl}";
        }

        return new Uri(new Uri(baseUrl), decodedUrl).ToString();
    }

    private static List<NiftyTick> ParseChartTicks(string html)
    {
        return ChartPointRegex
            .Matches(html)
            .Select(match =>
            {
                var year = int.Parse(match.Groups["year"].Value, CultureInfo.InvariantCulture);
                var zeroBasedMonth = int.Parse(match.Groups["month"].Value, CultureInfo.InvariantCulture);
                var day = int.Parse(match.Groups["day"].Value, CultureInfo.InvariantCulture);
                var hour = int.Parse(match.Groups["hour"].Value, CultureInfo.InvariantCulture);
                var minute = int.Parse(match.Groups["minute"].Value, CultureInfo.InvariantCulture);
                var price = decimal.Parse(match.Groups["price"].Value, CultureInfo.InvariantCulture);

                var chartTime = new DateTime(year, zeroBasedMonth + 1, day, hour, minute, 0);
                return new NiftyTick(chartTime, price);
            })
            .ToList();
    }
}

class ChartSource
{
    public ChartSource(string html, string url)
    {
        Html = html;
        Url = url;
    }

    public string Html { get; }
    public string Url { get; }
}

class NiftyTick
{
    public NiftyTick(DateTime time, decimal price)
    {
        Time = time;
        Price = price;
    }

    public DateTime Time { get; }
    public decimal Price { get; }
}

class NiftyResult
{
    public NiftyResult(
        string pageUrl,
        string chartSourceUrl,
        NiftyTick latest,
        DateTime targetTime,
        NiftyTick match,
        bool isExactMatch)
    {
        PageUrl = pageUrl;
        ChartSourceUrl = chartSourceUrl;
        Latest = latest;
        TargetTime = targetTime;
        Match = match;
        IsExactMatch = isExactMatch;
    }

    public string PageUrl { get; }
    public string ChartSourceUrl { get; }
    public NiftyTick Latest { get; }
    public DateTime TargetTime { get; }
    public NiftyTick Match { get; }
    public bool IsExactMatch { get; }
}
