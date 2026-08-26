using System.Globalization;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

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
    private const int FrameAttachTimeoutMs = 60_000;
    private const string InterstitialUrlMarker = "mc_interstitial_dfp.php";

    private static readonly Regex ChartPointRegex = new(
        @"Date\.UTC\((?<year>\d{4}),(?<month>\d{1,2}),(?<day>\d{1,2}),(?<hour>\d{1,2}),(?<minute>\d{1,2})\)\s*,\s*(?<price>\d+(?:\.\d+)?)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static async Task<NiftyResult> GetPriceFiveMinutesBeforeLatestAsync()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false
            });

        var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                Locale = "en-IN",
                TimezoneId = "Asia/Kolkata"
            });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(60_000);

        page.Popup += async (_, popup) =>
        {
            Console.WriteLine($"New popup opened: {popup.Url}");

            try
            {
                await popup.CloseAsync();
                Console.WriteLine("Popup closed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not close popup: {ex.Message}");
            }
        };

        Console.WriteLine("Opening Moneycontrol...");

        await page.GotoAsync(
            MarketActionPageUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60_000
            });

        Console.WriteLine("Moneycontrol page opened successfully.");

        await HandleInterstitialAsync(page);

        Console.WriteLine("Checking for Moneycontrol popups and advertisements...");

        await CloseAdvertisementsAndPopupsAsync(page);

        Console.WriteLine("Popup/ad handling completed.");
        Console.WriteLine("Waiting for the NIFTY 50 chart frame...");

        var chartSource = await GetNiftyChartHtmlAsync(page);

        var ticks = ParseChartTicks(chartSource.Html);

        if (ticks.Count == 0)
        {
            var snippet = Regex.Replace(chartSource.Html, @"\s+", " ");
            snippet = snippet.Length > 500 ? snippet[..500] : snippet;

            throw new InvalidOperationException(
                $"No NIFTY 50 chart points were found in the Moneycontrol chart source. " +
                $"Source: {chartSource.Url}. First HTML: {snippet}");
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

    private static async Task<ChartSource> GetNiftyChartHtmlAsync(IPage page)
    {
        var iframe = page.Locator(NiftyFrameSelector);

        try
        {
            await iframe.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Attached,
                    Timeout = FrameAttachTimeoutMs
                });
        }
        catch (TimeoutException)
        {
            throw new InvalidOperationException(
                $"NIFTY 50 iframe '{NiftyFrameSelector}' was not found on the Moneycontrol page. " +
                "Fallback URL functionality has been removed.");
        }

        var frame = page.Frames.FirstOrDefault(IsNiftyChartFrame);

        if (frame is null)
        {
            throw new InvalidOperationException(
                "NIFTY 50 chart frame was found in the DOM, but Playwright could not access the corresponding frame. " +
                "Fallback URL functionality has been removed.");
        }

        var html = await frame.ContentAsync();

        return new ChartSource(html, frame.Url);
    }

    private static bool IsNiftyChartFrame(IFrame frame)
    {
        return frame.Url.Contains(
                   "indices_chart.php",
                   StringComparison.OrdinalIgnoreCase)
               &&
               frame.Url.Contains(
                   "ind_id=9",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static async Task HandleInterstitialAsync(IPage page)
    {
        if (!page.Url.Contains(
                InterstitialUrlMarker,
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Console.WriteLine("Moneycontrol interstitial page detected.");

        var continueButton = page
            .Locator(".textlik")
            .GetByTitle("Moneycontrol");

        try
        {
            await continueButton.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5_000
                });

            Console.WriteLine("Clicking Continue to Moneycontrol...");

            await continueButton.ClickAsync();
            await page.WaitForTimeoutAsync(2_000);
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Moneycontrol Continue button was not found.");
        }
    }

    private static async Task CloseAdvertisementsAndPopupsAsync(IPage page)
    {
        await page.WaitForTimeoutAsync(2_000);

        string[] closeSelectors =
        {
            ".close",
            ".close-btn",
            ".closeBtn",
            ".close-button",
            ".ad-close",
            ".ad_close",
            ".popup-close",
            ".popup_close",
            ".modal-close",
            ".modal_close",
            "[aria-label='Close']",
            "[aria-label='close']",
            "[title='Close']",
            "[title='close']",
            "button:has-text('Close')"
        };

        foreach (var selector in closeSelectors)
        {
            try
            {
                var elements = page.Locator(selector);
                var count = await elements.CountAsync();

                for (var i = 0; i < count; i++)
                {
                    var element = elements.Nth(i);

                    if (!await element.IsVisibleAsync())
                    {
                        continue;
                    }

                    Console.WriteLine(
                        $"Closing advertisement/popup: {selector}");

                    await element.ClickAsync(
                        new LocatorClickOptions
                        {
                            Timeout = 2_000
                        });

                    await page.WaitForTimeoutAsync(300);
                }
            }
            catch
            {
                // Some ad elements disappear while iterating.
            }
        }

        try
        {
            await page.Keyboard.PressAsync("Escape");
        }
        catch
        {
            // Ignore if page is no longer available.
        }

        await page.WaitForTimeoutAsync(500);
    }

    private static List<NiftyTick> ParseChartTicks(string html)
    {
        return ChartPointRegex
            .Matches(html)
            .Select(match =>
            {
                var year = int.Parse(
                    match.Groups["year"].Value,
                    CultureInfo.InvariantCulture);

                var zeroBasedMonth = int.Parse(
                    match.Groups["month"].Value,
                    CultureInfo.InvariantCulture);

                var day = int.Parse(
                    match.Groups["day"].Value,
                    CultureInfo.InvariantCulture);

                var hour = int.Parse(
                    match.Groups["hour"].Value,
                    CultureInfo.InvariantCulture);

                var minute = int.Parse(
                    match.Groups["minute"].Value,
                    CultureInfo.InvariantCulture);

                var price = decimal.Parse(
                    match.Groups["price"].Value,
                    CultureInfo.InvariantCulture);

                var chartTime = new DateTime(
                    year,
                    zeroBasedMonth + 1,
                    day,
                    hour,
                    minute,
                    0);

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