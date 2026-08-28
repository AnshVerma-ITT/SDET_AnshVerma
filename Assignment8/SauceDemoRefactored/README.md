# SauceDemo Refactored POM Assignment

## Project Details

| Field | Value |
| --- | --- |
| Project Name | SauceDemo Refactored POM Assignment |
| Assignment | Assignment 1 - Refactor Week 6 Assignment Using POM |
| Application Type | Web UI Automation Testing Framework |
| Application Under Test | SauceDemo |
| Developed By | Ansh Verma |
| Language | C# |
| Runtime | .NET 10 |
| Test Framework | NUnit 4 |
| Automation Tool | Microsoft Playwright |
| Design Pattern | Page Object Model |
| Reporting | NUnit Attachments and Allure Results |
| Supported Browsers | Chromium and WebKit |

## Project Description

This project is a refactored version of the Week 6 SauceDemo UI automation assignment.

The framework uses Playwright with C#, NUnit, and the Page Object Model. Page locators and page actions are separated from test scenarios so the tests remain readable, reusable, and easy to maintain.

The project covers login, product sorting, product details, dynamic product selection, cart operations, checkout, negative validation, browser navigation, multiple pages, explicit waits, XPath relationships, parameterized tests, parallel execution, JSON configuration, trace capture, video recording, failure screenshots, and Allure reporting.

## Framework Summary

`Configuration` contains application routes, browser selection, browser parameterization, parallel settings, and JSON configuration loading.

`Pages` contains the Page Object Model classes for login, inventory, product details, cart, and checkout.

`Fixtures` contains reusable browser setup, browser creation, login flow, test evidence, and Allure helper code.

`TestData` contains credentials, products, customer information, page titles, and expected validation messages.

`Enums` and `Extensions` provide readable product sort options instead of repeating SauceDemo dropdown values in tests.

`Infrastructure` contains the reusable navigation command with configurable retry handling.

`Tests` contains the NUnit test scenarios. The XPath locator test intentionally keeps XPath examples in the test because its purpose is to demonstrate locator relationships.

## Refactoring Summary

Page locators and reusable page actions were moved into page object classes.

Application routes were moved to `AppRoutes`.

Credentials, product values, checkout information, page headings, and error messages were moved to dedicated test-data classes.

Common browser setup and teardown were moved to `TestBase`.

Browser creation was moved to `BrowserFactory`.

The repeated login journey was moved to `LoginFlow`.

Navigation retry behavior was moved to `NavigationHelper`.

Trace, video, screenshot, and attachment handling were moved to `TestEvidence`.

Product sorting values are represented by `ProductSortOption` and mapped by an extension method.

Browser selection is parameterized through `BrowserMatrix` and the `BROWSER` environment variable.

The project remains independent and does not reference the BDD or advanced-features projects.

## Project Structure

| Folder or File | Purpose |
| --- | --- |
| `Configuration` | Routes, browsers, settings, and parallel execution configuration. |
| `Enums` | Readable product sort choices. |
| `Extensions` | Maps sort choices to SauceDemo dropdown values. |
| `Fixtures` | Browser lifecycle, shared login flow, evidence, and Allure helpers. |
| `Infrastructure` | Reusable navigation command with retry handling. |
| `Pages` | Page Object Model classes and UI locators. |
| `TestData` | Centralized input data and expected values. |
| `Tests` | NUnit test fixtures and test cases. |
| `Reports` | Allure raw result output. |
| `TestResults` | Generated traces, videos, and failure screenshots. |
| `appsettings.json` | Runtime configuration. |
| `allureConfig.json` | Allure result configuration. |
| `SauceDemoRefactored.csproj` | Independent .NET 10 test project. |

## Page Object Files

| File | Purpose |
| --- | --- |
| `Pages/LoginPage.cs` | Opens the login page, enters credentials, submits login, and reads login errors. |
| `Pages/InventoryPage.cs` | Reads products, sorts products, adds or removes products, and opens product details or the cart. |
| `Pages/ProductDetailsPage.cs` | Validates product information and supports product-detail navigation. |
| `Pages/CartPage.cs` | Reads cart rows, validates products, continues shopping, and starts checkout. |
| `Pages/CheckoutPage.cs` | Enters customer information, validates checkout details, finishes an order, and reads validation messages. |

## Features Implemented

### 1. Page Object Model

- Separate page objects for each SauceDemo page.
- Locators are kept with their related page actions.
- Tests describe behavior instead of repeating UI interaction code.
- Shared login and navigation behavior is reused.

### 2. Authentication Testing

- Successful standard-user login.
- Invalid username.
- Invalid password.
- Missing username.
- Missing password.

### 3. Product Testing

- Sort by name from A to Z.
- Sort by name from Z to A.
- Sort by price from low to high.
- Sort by price from high to low.
- Open and validate product details.
- Select products dynamically using seeded random data.
- Return from product details using browser-back navigation.

### 4. Cart and Checkout Testing

- Add selected products and validate cart rows.
- Validate an empty cart.
- Add and validate every available product.
- Continue shopping from the cart.
- Complete the full purchase journey.
- Validate missing first name, last name, and postal code.

### 5. Additional Playwright and NUnit Coverage

- Chromium and WebKit parameterization.
- Parallel NUnit fixture execution.
- Multiple pages in one browser context.
- Explicit waiting.
- XPath parent, child, ancestor, descendant, and sibling relationships.
- Async and await.
- JSON configuration.
- Reusable navigation command with retry handling.
- Trace Viewer output.
- Video recording.
- Screenshot capture for failed tests.
- NUnit and Allure attachments.
- Intentional failure demonstration for evidence capture.

## Test Cases Implemented

| No. | Test Scenario | Test Type | Expected Result |
| --- | --- | --- | --- |
| 1 | Login with valid credentials | Positive | Inventory page is displayed. |
| 2 | Login with invalid username | Negative | Login error is displayed. |
| 3 | Login with invalid password | Negative | Login error is displayed. |
| 4 | Login without username | Negative | Username-required error is displayed. |
| 5 | Login without password | Negative | Password-required error is displayed. |
| 6 | Sort product names ascending | Positive | Names are displayed from A to Z. |
| 7 | Sort product names descending | Positive | Names are displayed from Z to A. |
| 8 | Sort prices ascending | Positive | Prices are displayed from low to high. |
| 9 | Sort prices descending | Positive | Prices are displayed from high to low. |
| 10 | Open product preview | Positive | Correct product details are displayed. |
| 11 | Add random products | Dynamic | Selected products are added to the cart. |
| 12 | Use browser back from details | Navigation | Inventory page is displayed again. |
| 13 | Validate selected cart products | Positive | Correct rows are present and shopping can continue. |
| 14 | Open an empty cart | Boundary | Cart contains zero products. |
| 15 | Add every product | Positive | Every SauceDemo product is present in the cart. |
| 16 | Complete checkout | End-to-end | Order confirmation is displayed. |
| 17 | Checkout without first name | Negative | First-name error is displayed. |
| 18 | Checkout without last name | Negative | Last-name error is displayed. |
| 19 | Checkout without postal code | Negative | Postal-code error is displayed. |
| 20 | Use multiple pages and an explicit wait | Browser context | Second page is handled successfully. |
| 21 | Locate elements using XPath relationships | Locator | Expected inventory elements are found. |
| 22 | Validate an intentionally wrong heading | Failure demonstration | Test fails and evidence is captured. |

There are 21 normal logical test cases and one intentional failure demonstration. Each logical case runs once for every browser selected by `BrowserMatrix`.

## Configuration Values

Configuration is stored in `appsettings.json`.

| Setting | Purpose |
| --- | --- |
| `baseUrl` | SauceDemo application URL. |
| `headless` | Controls visible or headless browser execution. |
| `timeoutMilliseconds` | Default Playwright action and navigation timeout. |
| `navigationMaxAttempts` | Maximum navigation attempts. |
| `navigationRetryDelayMilliseconds` | Delay between retry attempts. |
| `randomSeed` | Makes random product selection repeatable. |
| `screenshotOnFailure` | Enables screenshots for failed tests. |
| `traceEnabled` | Enables Playwright trace capture. |
| `videoEnabled` | Enables browser-context video recording. |

## C# and Testing Concepts Used

- Classes and objects
- Properties and methods
- Static classes
- Enums and extension methods
- Collections and read-only lists
- Async and await
- Loops and conditional statements
- Exception handling
- JSON deserialization
- Environment variables
- Page Object Model
- Separation of responsibilities
- DRY principle
- NUnit fixtures
- NUnit `Test`, `TestCase`, and `TestFixtureSource`
- Parameterization and parallel execution
- Playwright locators, assertions, pages, contexts, traces, and videos

## How To Run on Windows

### Prerequisites

- Windows 10 or Windows 11
- .NET 10 SDK
- Visual Studio or Visual Studio Code
- PowerShell

Open Windows PowerShell in the `Assignment8\SauceDemoRefactored` folder.

Confirm that .NET 10 is installed:

```powershell
dotnet --list-sdks
```

Restore packages:

```powershell
dotnet restore .\SauceDemoRefactored.csproj
```

Build the project:

```powershell
dotnet build .\SauceDemoRefactored.csproj
```

Install the Playwright browsers after the first build:

```powershell
powershell -ExecutionPolicy Bypass -File .\bin\Debug\net10.0\playwright.ps1 install chromium webkit
```

Run the 21 normal tests in Chromium:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\SauceDemoRefactored.csproj --filter "TestCategory!=FailureDemo"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run the 21 normal tests in WebKit:

```powershell
$env:BROWSER = "WebKit"
dotnet test .\SauceDemoRefactored.csproj --filter "TestCategory!=FailureDemo"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run the normal tests in both supported browsers:

```powershell
$env:BROWSER = "All"
dotnet test .\SauceDemoRefactored.csproj --filter "TestCategory!=FailureDemo"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run only the intentional failure demonstration in Chromium:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\SauceDemoRefactored.csproj --filter "TestCategory=FailureDemo"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run tests with detailed terminal output:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\SauceDemoRefactored.csproj --filter "TestCategory!=FailureDemo" --logger "console;verbosity=detailed"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

## Expected Terminal Output

When Chromium is selected and `FailureDemo` is excluded, NUnit should discover and pass 21 tests.

```text
Passed! - Failed: 0, Passed: 21, Skipped: 0, Total: 21
```

The `FailureDemo` test is intentionally expected to fail. It demonstrates screenshot, trace, video, NUnit attachment, and Allure evidence handling.

## Generated Evidence

| Output | Windows Location |
| --- | --- |
| Traces | `.\TestResults` |
| Videos | `.\TestResults` |
| Failure screenshots | `.\TestResults` |
| Allure raw results | `.\Reports\allure-results` |

## Expected Output

Valid users can log in to SauceDemo.

Invalid and incomplete login data displays the correct validation message.

Products can be sorted, opened, selected dynamically, added to the cart, and validated.

The complete cart and checkout journey succeeds.

Required checkout fields are validated.

Tests can run against Chromium, WebKit, or both browsers.

Traces, videos, screenshots, NUnit attachments, and Allure raw results are generated according to configuration.

## Conclusion

This project demonstrates how a direct UI test assignment can be refactored into a simple Page Object Model framework using Playwright, C#, NUnit, and .NET 10.

It improves readability and maintenance by separating page behavior, test data, configuration, browser setup, reusable flows, evidence handling, and test scenarios while preserving the Week 6 SauceDemo coverage.