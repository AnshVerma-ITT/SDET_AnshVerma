# Playwright Advanced Features Assignment

## Project Details

| Field | Value |
| --- | --- |
| Project Name | Playwright Advanced Features Assignment |
| Assignment | Supporting Project - Advanced Playwright Features |
| Application Type | Web UI Automation Feature Demonstration |
| Test Pages | Self-contained HTML created during each test |
| Developed By | Ansh Verma |
| Language | C# |
| Runtime | .NET 10 |
| Test Framework | NUnit 4 |
| Automation Tool | Microsoft Playwright |
| Reporting | NUnit Attachments and Allure Results |
| Supported Browsers | Chromium and WebKit |

## Project Description

This independent project demonstrates advanced Playwright browser features that are not available directly through the SauceDemo user interface.

The tests create small HTML pages inside the browser so file transfer, JavaScript dialogs, popups, iframes, JavaScript evaluation, parameterization, and API interception can be demonstrated without depending on another training website.

The project also demonstrates async and await, JSON configuration, browser selection, trace capture, video recording, failure screenshots, NUnit attachments, and Allure raw results.

## Framework Summary

`Tests` contains separate feature-focused classes for file transfer, alerts and popups, iframes, JavaScript evaluation, and API interception.

`BrowserTestBase` contains only NUnit setup and teardown. Browser creation and evidence handling are kept in separate fixture files.

`Configuration` contains browser names, the shared test category, and validated runtime values from `appsettings.json`.

`Infrastructure` contains reusable test-output path creation.

`Locators` contains every selector used by the advanced feature tests.

`TestData` contains the HTML pages, input values, expected values, API data, filenames, and JavaScript expression.

`TestAssets` contains the sample text file used by the upload test.

`Reports` stores Allure raw results.

Each test gets a new Playwright browser, browser context, and page. This keeps test state isolated and makes the examples easy to understand.

## Project Purpose

SauceDemo is suitable for login, product, cart, and checkout automation, but it does not provide every browser feature included in the syllabus.

This separate project provides small and direct examples for:

- Uploading and downloading a file.
- Handling a JavaScript alert.
- Handling a browser popup.
- Working with an iframe.
- Executing JavaScript with `EvaluateAsync()`.
- Parameterizing NUnit test cases.
- Intercepting and mocking an API call.
- Capturing traces, videos, failure screenshots, and Allure results.

The project does not reference the SauceDemo refactored or BDD projects.

## Project Structure

| Folder or File | Purpose |
| --- | --- |
| `Configuration` | Browser values, test category, and validated JSON settings. |
| `Fixtures/BrowserTestBase.cs` | Contains only NUnit setup and teardown. |
| `Fixtures/BrowserFactory.cs` | Launches the configured browser. |
| `Fixtures/TestEvidence.cs` | Captures traces, videos, screenshots, and attachments. |
| `Infrastructure/TestPaths.cs` | Creates paths below the .NET test-output folder. |
| `Locators` | Central Playwright selectors for all generated test pages. |
| `TestAssets/upload-sample.txt` | File selected by the upload test. |
| `TestData` | Central HTML, input, expected, file, JavaScript, and API values. |
| `Tests` | Five separate test classes grouped by feature type. |
| `Reports` | Allure raw result output. |
| `bin/Debug/net10.0/TestResults` | Generated downloads, traces, videos, and failure screenshots. |
| `appsettings.json` | Runtime configuration. |
| `allureConfig.json` | Allure result configuration. |
| `PlaywrightAdvancedFeatures.csproj` | Independent .NET 10 test project. |

## Features Implemented

### 1. File Upload

The test creates a file input and uploads `TestAssets\upload-sample.txt` using Playwright `SetInputFilesAsync()`.

The selected filename is validated after the upload action.

### 2. File Download

The test waits for the download event, downloads a text file, saves it to `TestResults`, validates the suggested filename, and reads the downloaded content.

### 3. Alert Handling

The test subscribes to the Playwright dialog event, reads the alert message, accepts the alert, and validates the captured text.

### 4. Popup Handling

The test waits for a new browser popup, writes content to the popup, validates its heading, and closes the popup.

### 5. Frame and IFrame Handling

The test uses `FrameLocator` to find a button inside an iframe, clicks it, and validates the updated text inside the same frame.

### 6. JavaScript Evaluation

The test passes integer values from C# to browser JavaScript and uses `EvaluateAsync<int>()` to calculate and return their total.

### 7. Test Parameterization

Two NUnit `TestCase` examples execute the JavaScript evaluation test with different input and expected values.

### 8. API Interception and Mocking

The test intercepts `**/api/products`, returns a controlled JSON response, performs a browser `fetch` request, validates the displayed mocked product, and confirms that interception occurred.

### 9. Evidence and Reporting

- Playwright trace for each test when enabled.
- Browser-context video for each test when enabled.
- Full-page screenshot when a test fails and screenshot capture is enabled.
- NUnit attachments.
- Allure raw results and attachments.

## Test Cases Implemented

| No. | Test Scenario | Playwright Feature | Expected Result |
| --- | --- | --- | --- |
| 1 | Upload and download files | File chooser and download event | Upload value, filename, and downloaded content are correct. |
| 2 | Handle alert and popup | Dialog and popup events | Alert is accepted and popup content is validated. |
| 3 | Interact with an iframe | `FrameLocator` | Text inside the frame changes successfully. |
| 4 | Evaluate values 2, 3, and 5 | `EvaluateAsync()` and `TestCase` | JavaScript returns 10. |
| 5 | Evaluate values 10, 20, and 30 | `EvaluateAsync()` and `TestCase` | JavaScript returns 60. |
| 6 | Intercept a product API call | `RouteAsync()` and `FulfillAsync()` | Mock product is displayed and interception is confirmed. |

## Configuration Values

Configuration is stored in `appsettings.json`.

| Setting | Purpose |
| --- | --- |
| `browser` | Default browser when `BROWSER` is not set. |
| `headless` | Controls visible or headless browser execution. |
| `timeoutMilliseconds` | Default Playwright action timeout. |
| `viewportWidth` | Browser viewport width. |
| `viewportHeight` | Browser viewport height. |
| `traceEnabled` | Enables Playwright trace capture. |
| `videoEnabled` | Enables browser-context video recording. |
| `screenshotOnFailure` | Enables screenshots for failed tests. |
| `evidenceDirectory` | Folder for downloads and Playwright evidence. |

The `BROWSER` environment variable can temporarily override the configured browser with `Chromium` or `WebKit`.

## C# and Testing Concepts Used

- Classes and inheritance
- Properties and methods
- Collections and dictionaries
- Async and await
- Events and event handlers
- Lambda expressions
- Raw string literals
- File and path handling
- JSON deserialization
- Environment variables
- Exception handling
- NUnit `SetUp` and `TearDown`
- NUnit `Test` and `TestCase`
- Parameterized testing
- Playwright pages and browser contexts
- Upload and download actions
- Dialog and popup events
- Frame locators
- JavaScript evaluation
- API route interception and response mocking
- Trace, video, screenshot, and attachment handling

## How To Run on Windows

### Prerequisites

- Windows 10 or Windows 11
- .NET 10 SDK
- Visual Studio or Visual Studio Code
- PowerShell

Open Windows PowerShell in the `Assignment8\PlaywrightAdvancedFeatures` folder.

Confirm that .NET 10 is installed:

```powershell
dotnet --list-sdks
```

Restore packages:

```powershell
dotnet restore .\PlaywrightAdvancedFeatures.csproj
```

Build the project:

```powershell
dotnet build .\PlaywrightAdvancedFeatures.csproj
```

Install the Playwright browsers after the first build:

```powershell
powershell -ExecutionPolicy Bypass -File .\bin\Debug\net10.0\playwright.ps1 install chromium webkit
```

Run all six test cases in Chromium:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\PlaywrightAdvancedFeatures.csproj --filter "TestCategory=AdvancedFeatures"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run all six test cases in WebKit:

```powershell
$env:BROWSER = "WebKit"
dotnet test .\PlaywrightAdvancedFeatures.csproj --filter "TestCategory=AdvancedFeatures"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run tests with detailed terminal output:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\PlaywrightAdvancedFeatures.csproj --filter "TestCategory=AdvancedFeatures" --logger "console;verbosity=detailed"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

## Expected Terminal Output

NUnit should discover six test cases because the JavaScript evaluation test has two parameterized examples.

```text
Passed! - Failed: 0, Passed: 6, Skipped: 0, Total: 6
```

## Generated Evidence

| Output | Windows Location |
| --- | --- |
| Downloaded file | `.\bin\Debug\net10.0\TestResults\playwright-example.txt` |
| Traces | `.\bin\Debug\net10.0\TestResults` |
| Videos | `.\bin\Debug\net10.0\TestResults` |
| Failure screenshots | `.\bin\Debug\net10.0\TestResults` |
| Allure raw results | `.\Reports\allure-results` |

## Expected Output

The sample file can be uploaded and a generated file can be downloaded and validated.

JavaScript alerts and browser popups are handled successfully.

Elements inside an iframe can be located and used.

JavaScript evaluation returns values to C# for both parameterized examples.

The product API request is intercepted and replaced with a controlled JSON response.

Trace, video, screenshot, NUnit attachment, and Allure evidence are generated according to configuration.

## Conclusion

This project demonstrates advanced browser automation using Playwright, C#, NUnit, and .NET 10 in a small and independent test framework.

It provides direct examples for file transfer, dialogs, popups, iframes, JavaScript evaluation, parameterization, API interception, asynchronous code, JSON configuration, and test reporting without adding unnecessary framework complexity.
