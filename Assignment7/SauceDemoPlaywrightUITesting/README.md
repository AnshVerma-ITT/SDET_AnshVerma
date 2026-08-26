# SauceDemo Playwright UI Testing Project

## Project Details

| Field | Value |
| --- | --- |
| Project Name | SauceDemo Playwright UI Testing Project |
| Developed By | Ansh Verma |
| Application Type | UI Test Automation Project |
| Website Under Test | SauceDemo |
| Language | C# |
| Platform | .NET 10 |
| Test Framework | NUnit |
| Automation Tool | Microsoft Playwright |
| Browsers | Chrome/Chromium and WebKit |
| Reporting Tool | Allure Report |

## Project Description

The SauceDemo Playwright UI Testing Project is a C# automation project developed to test the main user flows of the SauceDemo website.

The project uses Microsoft Playwright for browser automation, NUnit for writing and running tests, and Allure for test reporting. It follows the Page Object Model so page locators and actions remain separate from test cases.

The tests run on two browser engines only: Chrome/Chromium and WebKit. The browsers run in headed mode so the UI is visible during execution.

SauceDemo does not contain a Shadow DOM component. The separate local Shadow DOM fixture and test were removed as requested.

## Assignment Implementation Summary

- **Browser Testing:** Tests run on Chrome/Chromium and WebKit.
- **Page Object Model:** Login, inventory, product details, cart, and checkout pages have separate page classes.
- **Reusable Setup and Teardown:** Browser creation, context creation, screenshots, traces, and cleanup are managed in `TestBase`.
- **Configuration:** Base URL, headed mode, timeout, screenshot, trace, and Chrome channel settings are stored in `appsettings.json`.
- **Positive Testing:** Valid login, product sorting, cart behavior, product preview, and successful checkout are covered.
- **Negative Testing:** Invalid login and missing checkout information are covered.
- **Dynamic Locators:** Products are selected by their visible product names.
- **Playwright Features:** Role, text, placeholder, CSS, XPath, explicit wait, auto-waiting, dropdown, hover, keyboard, and multiple-page examples are included.
- **Failure Evidence:** Failed tests automatically create a full-page screenshot and Playwright trace.
- **Allure Reporting:** Test results, failure details, screenshots, and traces are included in the Allure report.
- **Intentional Failure:** A separate failure demonstration reads the real product heading and intentionally compares it with an incorrect expected value.

## Features Implemented

### 1. Login Testing

- Login with valid credentials
- Login with an invalid username
- Login with an invalid password
- Login with an empty username
- Login with an empty password
- Submit login using the Enter key

### 2. Product Testing

- Verify the Products page after login
- Sort products by name from A to Z
- Sort products by name from Z to A
- Sort products by price from low to high
- Sort products by price from high to low
- Open and verify a product preview page
- Add products using dynamic product-name locators

### 3. Cart Testing

- Add products to the cart
- Read repeated cart item rows
- Verify product name, quantity, and price
- Continue shopping from the cart
- Verify that the cart state is retained

### 4. Checkout Testing

- Enter customer checkout information
- Validate missing first name
- Validate missing last name
- Validate missing postal code
- Review the checkout summary
- Complete a purchase
- Verify the checkout completion message

### 5. Playwright Feature Testing

- CSS locators
- XPath `contains`
- XPath `parent`, `child`, `ancestor`, `descendant`, and `following-sibling` axes
- Role, text, and placeholder locators
- Dropdown selection
- Hover action
- Keyboard action
- Playwright auto-waiting
- Explicit waiting
- Multiple pages in one browser context

### 6. Reporting and Failure Evidence

- Allure result generation
- Allure HTML report generation
- NUnit failure details
- Full-page screenshot on failure
- Playwright trace for each test
- Intentional failed UI assertion for report demonstration

## Technologies and Concepts Used

- C#
- .NET 10
- NUnit
- Microsoft Playwright
- Allure NUnit
- Async and await
- Classes and objects
- Constructors
- Properties
- Inheritance
- Encapsulation
- Page Object Model
- Test fixtures
- Parameterized tests
- Test data classes
- Browser contexts
- Locators
- Assertions
- Setup and teardown
- Exception handling
- JSON configuration

## Project Structure

```text
SauceDemoPlaywrightUITesting
├── Configuration
├── Fixtures
├── Pages
├── Reports
│   └── allure-results
├── TestData
├── TestResults
├── Tests
├── allureConfig.json
├── appsettings.json
├── README.md
└── SauceDemo.Playwright.Tests.csproj
```

## Initial Project Setup

Run these commands from the project root the first time:

```bash
dotnet restore
dotnet build
```

Install the Chrome/Chromium and WebKit browser binaries:

## How To Run Tests

Run the normal test suite on both browsers:

```bash
dotnet test --filter "TestCategory!=FailureDemo"
```

Run the normal tests on Chrome only:

```bash
$env:BROWSER="Chrome"; dotnet test --filter "TestCategory!=FailureDemo"
```

Run the normal tests on WebKit only:

```bash
BROWSER=WebKit dotnet test --filter "TestCategory!=FailureDemo"
```

The browsers are visible because `headless` is set to `false` in `appsettings.json`.

## How To Run The Intentional Failure

Run the failure demonstration on Chrome:

```bash
BROWSER=Chrome dotnet test --filter "TestCategory=FailureDemo"
```

This command is expected to finish with one failed test. The test reads the real heading `Products` from SauceDemo and intentionally expects `Product Catalog`. It uses a normal NUnit assertion rather than calling `Assert.Fail`.

The failed command returning exit code `1` is correct for this demonstration.

## How Allure Reporting Works

Allure reporting has three separate parts:

1. `Reports/allure-results` contains raw JSON result files, PNG attachments, and ZIP trace attachments created during `dotnet test`.
2. `allure-report` contains the generated HTML report files.
3. The Allure local server displays the generated report correctly in a browser.

Create a fresh complete report by running the following commands one at a time:

```bash
Remove-Item -Recurse -Force ".\Reports\allure-results", ".\allure-report"
dotnet test --filter "TestCategory!=FailureDemo"
BROWSER=Chrome dotnet test --filter "TestCategory=FailureDemo"
npx.cmd --yes allure-commandline generate ".\Reports\allure-results" --clean -o ".\allure-report"
npx.cmd --yes allure-commandline open ".\allure-report"
```

The intentional failure command returns exit code `1`. Continue with the Allure commands after that expected failure. The complete report will contain the normal tests and the intentional failed test.

Generate the HTML report after running tests:

```bash
npx.cmd --yes allure-commandline generate ".\Reports\allure-results" --clean -o ".\allure-report"
```

Open the report through the Allure local server:

```bash
npx.cmd --yes allure-commandline open ".\allure-report"
```

Do not open `allure-report/index.html` directly. A direct `file://` page cannot fetch the report data correctly and may display `500 Failed to fetch`.

In the report, open the failed test and view these attachments:

- `Failure screenshot`
- `Playwright trace`

The same failure evidence is also stored in the project-level `TestResults` folder.

## Debug With Playwright Inspector

Run one login test with the Playwright Inspector:

```bash
PWDEBUG=1 BROWSER=Chrome dotnet test --filter "Name=Login_WithValidCredentials_ShouldSucceed"
```

The Inspector allows the test to be resumed one step at a time and includes a locator picker.

## Expected Output

- Chrome/Chromium and WebKit browser windows are visible during normal test execution.
- The normal suite passes when the `FailureDemo` category is excluded.
- The intentional failure command reports one failed test.
- A real screenshot of the SauceDemo Products page is created for the intentional failure.
- A Playwright trace ZIP file is created.
- Raw Allure results are written to `Reports/allure-results`.
- The generated Allure report shows the test as failed rather than broken.
- The failed test contains the screenshot and trace attachments.

## Conclusion

This project demonstrates UI test automation using C#, NUnit, and Microsoft Playwright. It covers the main SauceDemo login, product, cart, and checkout workflows on Chrome/Chromium and WebKit. It also demonstrates reusable browser setup, the Page Object Model, multiple locator types, waits, parameterized tests, failure screenshots, Playwright traces, and Allure reporting.