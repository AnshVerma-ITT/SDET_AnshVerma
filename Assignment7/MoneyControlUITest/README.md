# Moneycontrol NIFTY 50 Playwright Price Reader

## Project Details

| Field              | Value                                         |
| ------------------ | --------------------------------------------- |
| Project Name       | Moneycontrol NIFTY 50 Playwright Price Reader |
| Developed By       | Ansh Verma                                    |
| Application Type   | Browser Automation / Data Extraction Project  |
| Website Under Test | Moneycontrol                                  |
| Target Data        | NIFTY 50 Index Price                          |
| Language           | C#                                            |
| Platform           | .NET 10                                       |
| Automation Tool    | Microsoft Playwright                          |
| Browser            | Chromium                                      |
| Browser Mode       | Headed                                        |
| Time Zone          | Asia/Kolkata                                  |
| Data Source        | Moneycontrol NIFTY 50 Chart iframe            |

## Project Description

The Moneycontrol NIFTY 50 Playwright Price Reader is a C# browser automation project that uses Microsoft Playwright to open the Moneycontrol website and retrieve NIFTY 50 chart data.

The project identifies the NIFTY 50 chart iframe on the Moneycontrol page, accesses the chart frame using Playwright, reads the chart HTML, extracts timestamp and price values using a regular expression, and determines the NIFTY 50 price from approximately five minutes before the latest available chart tick.

The project runs Chromium in headed mode so that the browser activity can be observed during execution.

The implementation also handles Moneycontrol interstitial pages, advertisements, popups, and dynamically attached iframe content.

## Assignment Implementation Summary

* **Browser Automation:** Chromium is launched using Microsoft Playwright.

* **Headed Execution:** The browser runs with `Headless = false`, allowing the UI to remain visible during execution.

* **Moneycontrol Navigation:** The automation opens the Moneycontrol Indian stocks and markets page.

* **Interstitial Handling:** The project detects a Moneycontrol interstitial page and attempts to click the Continue button.

* **Popup Handling:** Newly opened browser popups are detected and closed automatically.

* **Advertisement Handling:** Multiple common advertisement and popup close selectors are checked and closed when visible.

* **NIFTY 50 iframe Handling:** The project waits for the NIFTY 50 chart iframe `iframe#nif_load_graph` to be attached.

* **Frame Identification:** The correct chart frame is identified using the `indices_chart.php` URL and `ind_id=9` parameter.

* **Chart HTML Extraction:** Playwright retrieves the complete HTML content of the NIFTY 50 chart frame.

* **Regex Data Extraction:** Chart timestamps and prices are extracted from the chart HTML using a compiled regular expression.

* **Timestamp Conversion:** JavaScript `Date.UTC()` chart timestamps are converted into C# `DateTime` values.

* **Latest Tick Detection:** All extracted chart ticks are sorted chronologically and the latest available tick is identified.

* **Five-Minute Calculation:** The target time is calculated by subtracting five minutes from the latest available chart time.

* **Exact Match:** If a chart tick exists exactly five minutes before the latest tick, that tick is selected.

* **Nearest Earlier Match:** If an exact five-minute tick does not exist, the latest available tick before the target time is selected.

* **Fallback Match:** If no earlier tick is available, the earliest available chart tick is returned.

* **Culture Handling:** `CultureInfo.InvariantCulture` is used for reliable parsing of numeric values.

* **Error Handling:** Meaningful exceptions are thrown when the iframe, chart frame, or chart data cannot be found.

* **Structured Results:** `NiftyTick`, `ChartSource`, and `NiftyResult` classes are used to represent extracted data and the final result.

## Features Implemented

### 1. Moneycontrol Page Navigation

* Open the Moneycontrol stocks and markets page

* Wait for the DOM content to load

* Configure a 60-second default Playwright timeout

* Use the `Asia/Kolkata` time zone

* Use the `en-IN` browser locale

* Display browser activity in headed mode

### 2. Interstitial Page Handling

The project checks whether Moneycontrol redirects the browser to an interstitial page.

The following URL marker is used:

```text
mc_interstitial_dfp.php
```

If the interstitial is detected:

* The project searches for the Moneycontrol Continue button

* Waits for the button to become visible

* Clicks the Continue button

* Waits briefly for the Moneycontrol page to load

* Continues execution if the button is unavailable

### 3. Popup Handling

Playwright's `Page.Popup` event is used to detect newly opened browser popups.

When a popup is opened:

* Its URL is printed to the console

* The popup is closed automatically

* Any popup closing exception is handled safely

Example console output:

```text
New popup opened: <popup-url>
Popup closed successfully.
```

### 4. Advertisement and Popup Closing

The project checks multiple commonly used selectors for advertisements, modal windows, and close buttons.

Selectors include:

```text
.close
.close-btn
.closeBtn
.close-button
.ad-close
.ad_close
.popup-close
.popup_close
.modal-close
.modal_close
[aria-label='Close']
[aria-label='close']
[title='Close']
[title='close']
button:has-text('Close')
```

Visible elements are clicked and closed.

The project also sends the `Escape` keyboard key after attempting to close advertisements.

### 5. NIFTY 50 Chart iframe

The NIFTY 50 chart is loaded inside an iframe.

The project identifies the iframe using:

```css
iframe#nif_load_graph
```

The iframe is first waited on using:

```csharp
await iframe.WaitForAsync(
    new LocatorWaitForOptions
    {
        State = WaitForSelectorState.Attached,
        Timeout = FrameAttachTimeoutMs
    });
```

The timeout is:

```text
60 seconds
```

### 6. NIFTY 50 Frame Identification

After the iframe is attached, Playwright's available frames are inspected.

The required frame must contain:

```text
indices_chart.php
```

and:

```text
ind_id=9
```

The frame is identified using:

```csharp
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
```

This ensures that the project accesses the intended NIFTY 50 chart frame.

### 7. Chart HTML Extraction

Once the correct frame is found, the complete frame HTML is retrieved using:

```csharp
var html = await frame.ContentAsync();
```

The returned HTML and frame URL are stored in a `ChartSource` object.

### 8. Chart Data Parsing

The chart data is extracted from the HTML using a compiled regular expression.

The regular expression searches for chart values in the following general format:

```text
Date.UTC(year,month,day,hour,minute),price
```

Example structure:

```text
Date.UTC(2026,7,26,10,30),24950.25
```

The regex captures:

* Year

* Month

* Day

* Hour

* Minute

* Price

### 9. Timestamp Conversion

The chart uses a zero-based month value because the source is JavaScript-based.

For example:

```text
0 = January
1 = February
2 = March
...
```

The project therefore adds one to the extracted month:

```csharp
var chartTime = new DateTime(
    year,
    zeroBasedMonth + 1,
    day,
    hour,
    minute,
    0);
```

### 10. Price Parsing

The extracted price is converted into a C# `decimal`:

```csharp
var price = decimal.Parse(
    match.Groups["price"].Value,
    CultureInfo.InvariantCulture);
```

Using `decimal` helps preserve the numerical precision required for price values.

### 11. Latest Chart Tick

All extracted chart ticks are ordered by their timestamp:

```csharp
var orderedTicks = ticks
    .OrderBy(tick => tick.Time)
    .ToList();
```

The final tick is treated as the latest available chart data:

```csharp
var latest = orderedTicks[^1];
```

### 12. Five-Minute Target Calculation

The requested time is calculated by subtracting five minutes from the latest chart timestamp:

```csharp
var targetTime = latest.Time.AddMinutes(-5);
```

For example:

```text
Latest chart time : 2026-08-26 11:30
Requested time    : 2026-08-26 11:25
```

### 13. Exact Five-Minute Match

The project first searches for a chart tick whose timestamp exactly matches the requested time:

```csharp
var exactMatch = orderedTicks
    .FirstOrDefault(tick => tick.Time == targetTime);
```

If an exact match exists, it is returned.

### 14. Nearest Earlier Tick

If there is no exact five-minute match, the project searches for the latest available tick before the target time:

```csharp
orderedTicks.LastOrDefault(
    tick => tick.Time <= targetTime)
```

This handles cases where the chart does not contain a tick at exactly five minutes before the latest value.

The console output identifies this situation using:

```text
(nearest earlier available tick)
```

### 15. Result Object

The final result is stored in the `NiftyResult` class.

It contains:

| Property         | Description                              |
| ---------------- | ---------------------------------------- |
| `PageUrl`        | Moneycontrol page URL                    |
| `ChartSourceUrl` | URL of the NIFTY 50 chart frame          |
| `Latest`         | Latest available chart tick              |
| `TargetTime`     | Time five minutes before the latest tick |
| `Match`          | Exact or nearest earlier matching tick   |
| `IsExactMatch`   | Indicates whether the match was exact    |

## Technologies and Concepts Used

* C#

* .NET 10

* Microsoft Playwright

* Chromium

* Browser automation

* Async and await

* `IPage`

* `IFrame`

* Playwright Locators

* Playwright Frames

* Playwright Events

* Popup handling

* Keyboard actions

* Explicit waiting

* Playwright auto-waiting

* CSS selectors

* XPath-compatible selector concepts

* Regular expressions

* `Regex`

* `CultureInfo.InvariantCulture`

* `decimal` parsing

* LINQ

* `OrderBy`

* `FirstOrDefault`

* `LastOrDefault`

* `DateTime`

* `DateTime.AddMinutes()`

* Classes and objects

* Constructors

* Properties

* Exception handling

* Browser contexts

* Time zone configuration

## Project Structure

```text
MoneycontrolNifty50PriceReader

├── Program.cs
├── README.md
└── <ProjectName>.csproj
```

The main implementation is contained in a single C# source file.

The file contains:

```text
Program
│
├── Nifty50PriceReader
│   ├── GetPriceFiveMinutesBeforeLatestAsync()
│   ├── GetNiftyChartHtmlAsync()
│   ├── IsNiftyChartFrame()
│   ├── HandleInterstitialAsync()
│   ├── CloseAdvertisementsAndPopupsAsync()
│   └── ParseChartTicks()
│
├── ChartSource
│
├── NiftyTick
│
└── NiftyResult
```

## Main Execution Flow

The complete execution flow is:

```text
Start
  |
  v
Create Playwright
  |
  v
Launch Chromium
  |
  v
Create Browser Context
  |
  v
Create Page
  |
  v
Open Moneycontrol
  |
  v
Handle Interstitial
  |
  v
Handle Popups and Advertisements
  |
  v
Wait for NIFTY 50 iframe
  |
  v
Find NIFTY 50 chart frame
  |
  v
Read Chart HTML
  |
  v
Parse Chart Ticks
  |
  v
Sort Ticks by Time
  |
  v
Find Latest Tick
  |
  v
Subtract 5 Minutes
  |
  v
Find Exact Match
  |
  +---- Match Found ----> Return Exact Tick
  |
  +---- No Match -------> Find Nearest Earlier Tick
  |
  v
Display Result
  |
  v
End
```

## Initial Project Setup

Run the following commands from the project root.

### Restore Dependencies

```bash
dotnet restore
```

### Build the Project

```bash
dotnet build
```

### Install Playwright Browser

If Playwright browser binaries have not already been installed, build the project first and then install Chromium.

For PowerShell:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

If PowerShell execution policy prevents the script from running, the Playwright installation command can be executed through an appropriate PowerShell policy configuration permitted by the development environment.

## How To Run The Application

Run the application using:

```bash
dotnet run
```

The Chromium browser will open in headed mode.

The application will navigate to Moneycontrol and process the NIFTY 50 chart.

## Expected Console Output

A successful execution produces output similar to:

```text
Opening Moneycontrol...
Moneycontrol page opened successfully.
Checking for Moneycontrol popups and advertisements...
Popup/ad handling completed.
Waiting for the NIFTY 50 chart frame...

Moneycontrol NIFTY 50 data retrieved from browser-inspected chart data
Opened page          : https://www.moneycontrol.com/stocksmarketsindia/
Chart data source    : <NIFTY-50-chart-frame-url>
Latest chart time    : 2026-08-26 11:30
Latest chart price   : 24,950.25
Requested chart time : 2026-08-26 11:25
Matched chart time   : 2026-08-26 11:25
NIFTY 50 price       : 24,940.10
```

The actual timestamp and price values depend on the chart data available when the application is executed.

## Understanding The Five-Minute Calculation

The application does not simply subtract five minutes from the current computer time and search for a value.

Instead, it works with the timestamps actually available in the Moneycontrol chart.

The calculation is:

```text
Latest available chart tick
          |
          v
Subtract 5 minutes
          |
          v
Target chart time
          |
          v
Search chart data
          |
          +--> Exact timestamp exists
          |        |
          |        v
          |   Return exact tick
          |
          +--> Exact timestamp does not exist
                   |
                   v
             Find latest tick
             before target time
                   |
                   v
               Return tick
```

This approach is useful because chart data may not always contain a tick at exactly the requested five-minute timestamp.

## Example

Suppose the latest available chart data is:

```text
Latest:
11:30 -> 24,950.25
```

The application calculates:

```text
11:30 - 5 minutes = 11:25
```

If the chart contains:

```text
11:20 -> 24,930.10
11:25 -> 24,940.10
11:30 -> 24,950.25
```

then:

```text
Target Time : 11:25
Matched Time: 11:25
Price       : 24,940.10
```

If the chart instead contains:

```text
11:20 -> 24,930.10
11:24 -> 24,938.50
11:30 -> 24,950.25
```

then there is no exact `11:25` tick.

The application selects:

```text
Matched Time: 11:24
Price       : 24,938.50
```

and reports it as:

```text
(nearest earlier available tick)
```

## Error Handling

The project contains explicit error handling for important failure scenarios.

### NIFTY 50 iframe Not Found

If the expected iframe does not appear within the configured timeout, the application throws an exception explaining that the NIFTY 50 iframe could not be found.

Expected error structure:

```text
NIFTY 50 iframe 'iframe#nif_load_graph' was not found on the Moneycontrol page.
```

### Chart Frame Not Accessible

If the iframe exists but Playwright cannot access the corresponding NIFTY 50 frame, an exception is thrown.

```text
NIFTY 50 chart frame was found in the DOM, but Playwright could not access the corresponding frame.
```

### No Chart Data Found

If the chart HTML is successfully retrieved but no matching chart points are found, the application throws an exception.

The error includes:

* Chart source URL

* A shortened portion of the returned HTML

This helps with debugging changes to the Moneycontrol chart structure.

## Why Playwright Frames Are Used

The NIFTY 50 chart is not part of the main page DOM.

It is loaded inside an iframe.

Therefore, simply using:

```csharp
page.Locator(...)
```

on the main page is not sufficient for accessing the chart's internal HTML.

The project first identifies the iframe and then accesses the corresponding `IFrame` object:

```csharp
var frame = page.Frames.FirstOrDefault(IsNiftyChartFrame);
```

The chart HTML can then be retrieved using:

```csharp
var html = await frame.ContentAsync();
```

This demonstrates Playwright's frame-handling capability.

## Why Regular Expression Is Used

The chart HTML contains timestamp and price information in JavaScript chart data.

The project uses a regular expression to identify values in the chart source.

The regex extracts:

```text
Year
Month
Day
Hour
Minute
Price
```

The extracted values are then converted into strongly typed C# values:

```text
Year/Month/Day/Time -> DateTime
Price               -> decimal
```

This separates the raw HTML representation from the application's structured data.

## Browser Configuration

The project creates the browser context using:

```csharp
new BrowserNewContextOptions
{
    Locale = "en-IN",
    TimezoneId = "Asia/Kolkata"
}
```

This configuration makes the browser environment appropriate for Indian market data.

Chromium is launched with:

```csharp
new BrowserTypeLaunchOptions
{
    Headless = false
}
```

Therefore, the browser remains visible during execution.

## Timeout Configuration

The project uses a default Playwright timeout of:

```text
60,000 milliseconds
```

This is configured using:

```csharp
page.SetDefaultTimeout(60_000);
```

The NIFTY 50 iframe also has a dedicated attachment timeout:

```text
60,000 milliseconds
```

This gives the dynamically loaded chart sufficient time to become available.

## Important Constants

The application contains the following important configuration values:

| Constant                | Purpose                                         |
| ----------------------- | ----------------------------------------------- |
| `MarketActionPageUrl`   | Moneycontrol page opened by the application     |
| `NiftyFrameSelector`    | CSS selector used to locate the NIFTY 50 iframe |
| `FrameAttachTimeoutMs`  | Maximum wait time for the chart iframe          |
| `InterstitialUrlMarker` | Identifies Moneycontrol interstitial pages      |

The main values are:

```text
MarketActionPageUrl = https://www.moneycontrol.com/stocksmarketsindia/

NiftyFrameSelector = iframe#nif_load_graph

FrameAttachTimeoutMs = 60000

InterstitialUrlMarker = mc_interstitial_dfp.php
```

## Data Models

### ChartSource

`ChartSource` stores the source HTML and URL of the NIFTY 50 chart frame.

Properties:

```text
Html
Url
```

### NiftyTick

`NiftyTick` represents one chart data point.

Properties:

```text
Time
Price
```

Example:

```text
Time  : 2026-08-26 11:25
Price : 24940.10
```

### NiftyResult

`NiftyResult` represents the complete result of the five-minute lookup.

Properties:

```text
PageUrl
ChartSourceUrl
Latest
TargetTime
Match
IsExactMatch
```

## Important Playwright Concepts Demonstrated

### Browser Creation

```csharp
using var playwright = await Playwright.CreateAsync();

await using var browser =
    await playwright.Chromium.LaunchAsync(...);
```

### Browser Context

```csharp
var context = await browser.NewContextAsync(...);
```

### Page Creation

```csharp
var page = await context.NewPageAsync();
```

### Page Navigation

```csharp
await page.GotoAsync(...);
```

### Locator

```csharp
var iframe = page.Locator("iframe#nif_load_graph");
```

### Frame Access

```csharp
var frame = page.Frames.FirstOrDefault(IsNiftyChartFrame);
```

### Frame HTML

```csharp
var html = await frame.ContentAsync();
```

### Popup Event

```csharp
page.Popup += async (_, popup) =>
{
    ...
};
```

### Keyboard Action

```csharp
await page.Keyboard.PressAsync("Escape");
```

### Explicit Wait

```csharp
await iframe.WaitForAsync(...);
```

### Async/Await

All browser operations are asynchronous:

```csharp
await page.GotoAsync(...);

await frame.ContentAsync();

await popup.CloseAsync();
```

## Limitations

* The project depends on the current structure of the Moneycontrol website.

* Changes to the NIFTY 50 iframe ID may require changes to `NiftyFrameSelector`.

* Changes to the chart frame URL or `ind_id=9` parameter may require changes to `IsNiftyChartFrame()`.

* Changes to the JavaScript chart data format may require changes to `ChartPointRegex`.

* Advertisement selectors can change as Moneycontrol updates its website.

* The application depends on chart data being available when the page is loaded.

* The application uses the latest available chart tick rather than assuming that the computer's current time is the latest chart timestamp.

* The returned five-minute value can be a nearest earlier tick when an exact five-minute tick does not exist.

## Troubleshooting

### Playwright Browser Is Not Installed

Run:

```bash
dotnet build
```

and then install Chromium using the generated Playwright script.

### PowerShell Execution Policy Error

If PowerShell displays an error similar to:

```text
running scripts is disabled on this system
```

the issue is related to the PowerShell execution policy rather than the C# application itself.

Use an approved PowerShell execution-policy approach for the development environment or execute the Playwright installation command through an allowed shell.

### NIFTY 50 iframe Is Not Found

Possible causes:

* Moneycontrol changed the page structure.

* The iframe ID changed.

* The page did not finish loading.

* A new interstitial or blocking page appeared.

* Network conditions delayed the iframe.

Check:

```text
iframe#nif_load_graph
```

### No Chart Points Found

Possible causes:

* Moneycontrol changed the chart implementation.

* The chart no longer contains the expected `Date.UTC()` format.

* The chart has not loaded completely.

* The selected frame is no longer the NIFTY 50 frame.

Check the exception output for the chart source URL and HTML snippet.

## Running The Project

The basic workflow is:

```bash
dotnet restore
dotnet build
dotnet run
```

The browser will open automatically because the project is configured for headed execution.

## Expected Result

After successful execution, the console displays:

```text
Moneycontrol NIFTY 50 data retrieved from browser-inspected chart data
Opened page          : <Moneycontrol URL>
Chart data source    : <NIFTY 50 chart URL>
Latest chart time    : <latest timestamp>
Latest chart price   : <latest price>
Requested chart time : <latest timestamp - 5 minutes>
Matched chart time   : <matched timestamp>
NIFTY 50 price       : <matched price>
```

If an exact five-minute chart tick does not exist, the matched time is followed by:

```text
(nearest earlier available tick)
```

## Conclusion

This project demonstrates browser automation and chart-data extraction using C# and Microsoft Playwright.

It covers navigation to Moneycontrol, interstitial handling, popup and advertisement handling, iframe identification, Playwright frame interaction, chart HTML extraction, regular-expression parsing, timestamp conversion, decimal price parsing, LINQ-based data processing, and five-minute historical price matching.

The main purpose of the project is to retrieve the NIFTY 50 price corresponding to five minutes before the latest available chart tick while handling the dynamic nature of the Moneycontrol website.
