# SauceDemo BDD Assignment

## Project Details

| Field | Value |
| --- | --- |
| Project Name | SauceDemo BDD Assignment |
| Assignment | Assignment 2 - SauceDemo POM with BDD |
| Application Type | BDD Web UI Automation Testing Framework |
| Application Under Test | SauceDemo |
| Developed By | Ansh Verma |
| Language | C# |
| Runtime | .NET 10 |
| Test Framework | NUnit 4 |
| Automation Tool | Microsoft Playwright |
| BDD Tool | Reqnroll |
| BDD Language | Gherkin |
| Design Pattern | Page Object Model |
| Reporting | NUnit Attachments and Allure Results |
| Supported Browsers | Chromium and WebKit |

## Project Description

This project is an independent SauceDemo automation framework created for Assignment 2.

It applies the Page Object Model and Behavior-Driven Development to the Week 6 SauceDemo purchase scenario. Business behavior is written in Gherkin feature files, executable steps are implemented through Reqnroll bindings, page locators and actions are stored in page object classes, and NUnit runs the generated scenarios.

The assignment covers successful and unsuccessful login behavior, product sorting, adding and removing products, cart persistence, the complete purchase journey, and checkout validation. It also demonstrates parameterized scenario outlines, parallel feature execution, async and await, JSON configuration, trace capture, video recording, failure screenshots, and Allure reporting.

## What Is BDD?

Behavior-Driven Development describes expected application behavior using examples that can be understood by testers, developers, and business users.

Gherkin expresses those examples using readable keywords:

| Keyword | Purpose |
| --- | --- |
| `Feature` | Describes the business capability under test. |
| `Background` | Runs shared preparation before each scenario in a feature. |
| `Scenario` | Describes one executable behavior example. |
| `Scenario Outline` | Runs the same behavior with multiple example values. |
| `Examples` | Supplies values to a scenario outline. |
| `Given` | Defines the starting condition. |
| `When` | Defines the user action. |
| `Then` | Defines the expected result. |

## Framework Summary

`Features` contains the authentication, inventory/cart, and checkout behavior written in Gherkin.

`StepDefinitions` connects each Given, When, and Then statement to executable C# code.

`Pages` contains the Page Object Model classes. Locators are kept inside these classes instead of being repeated in step definitions.

`Flows` contains the reusable standard-user login journey.

`Support` contains the browser driver, Reqnroll hooks, scenario-specific state, evidence capture, and resource cleanup.

`Configuration` contains application routes, parallel execution settings, and JSON configuration loading and validation.

`TestData` contains credentials, product aliases, customer information, expected confirmation text, and validation errors.

Reqnroll generates NUnit tests from the `.feature` files during the build. The `@bdd` tag becomes the `bdd` NUnit category used by the test command.

## Assignment Completion Summary

The complete Week 6 purchase journey is automated as a readable BDD scenario.

Authentication, inventory/cart, and checkout behavior are separated into logical feature files.

Page actions and locators are separated from step definitions through the Page Object Model.

Repeated standard-user login behavior is implemented once in `LoginFlow`.

Routes, credentials, products, customer values, expected errors, browser settings, paths, and test-id configuration are centralized.

Scenario outlines parameterize five unsuccessful login examples and three incomplete checkout examples.

Scenario data is isolated through `ScenarioState`, preventing data from being shared between scenarios.

Browser setup and teardown are handled by Reqnroll hooks.

Chromium and WebKit can be selected without changing source code.

The project remains independent and does not reference the refactored or advanced-features projects.

## Project Structure

| Folder or File | Purpose |
| --- | --- |
| `Configuration` | Routes, settings loading, validation, and parallel configuration. |
| `Features` | Gherkin feature files and Allure feature attributes. |
| `Flows` | Reusable multi-page user journeys. |
| `Pages` | Page Object Model classes and locators. |
| `StepDefinitions` | Given, When, and Then bindings. |
| `Support` | Browser driver, scenario hooks, state, and evidence handling. |
| `TestData` | Central credentials, products, checkout data, and expected messages. |
| `Reports` | Allure raw result output. |
| `appsettings.json` | Runtime configuration. |
| `allureConfig.json` | Allure result configuration. |
| `SauceDemoBDD.csproj` | Independent .NET 10 BDD test project. |

## Feature Files

| File | Purpose | Executed Scenarios |
| --- | --- | --- |
| `Features/Authentication.feature` | Successful login and unsuccessful credential validation. | 6 |
| `Features/InventoryAndCart.feature` | Product sorting, cart retention, and product removal. | 3 |
| `Features/Checkout.feature` | Complete purchase flow and checkout-field validation. | 4 |

## Page Object Files

| File | Purpose |
| --- | --- |
| `Pages/LoginPage.cs` | Opens login, enters credentials, submits login, and reads errors. |
| `Pages/InventoryPage.cs` | Sorts products, reads prices, adds or removes exact products, and opens the cart. |
| `Pages/CartPage.cs` | Validates exact cart products, counts rows, continues shopping, and starts checkout. |
| `Pages/CheckoutPage.cs` | Enters customer data, reads validation errors, validates overview items, and completes an order. |

## Step Definition Files

| File | Purpose |
| --- | --- |
| `StepDefinitions/LoginSteps.cs` | Login-page, valid-login, invalid-login, and inventory-result steps. |
| `StepDefinitions/InventorySteps.cs` | Sorting, adding, removing, badge-count, and cart-navigation steps. |
| `StepDefinitions/CartSteps.cs` | Selected-product, cart-count, continue-shopping, and checkout-start steps. |
| `StepDefinitions/CheckoutSteps.cs` | Customer-information, checkout-error, overview, finish, and confirmation steps. |

## Features Implemented

### 1. Authentication Behavior

- Successful standard-user login.
- Invalid username.
- Invalid password.
- Missing username.
- Missing password.
- Locked user.

### 2. Inventory and Cart Behavior

- Sort products by price from low to high.
- Add multiple products using a Gherkin table.
- Validate the cart badge count.
- Validate exact products in the cart.
- Continue shopping without losing cart contents.
- Remove a product and validate the updated badge.

### 3. Complete Week 6 Purchase Journey

- Log in with the standard user.
- Add the backpack and bike light.
- Open the cart and validate both products.
- Continue shopping and retain the cart.
- Begin checkout.
- Submit valid customer information.
- Validate the checkout overview product count.
- Finish the order.
- Validate the order confirmation.

### 4. Checkout Validation

- Missing first name.
- Missing last name.
- Missing postal code.

### 5. Framework and Reporting Coverage

- Page Object Model.
- Gherkin features and scenarios.
- Background steps.
- Scenario outlines and examples.
- Gherkin data tables.
- Reqnroll integration with NUnit.
- Async and await.
- Parallel feature execution with two workers.
- JSON configuration.
- Configurable Chromium or WebKit execution.
- Configurable test-id attribute.
- Scenario-specific state.
- Trace Viewer output.
- Video recording.
- Screenshot capture for failed scenarios.
- NUnit and Allure attachments.

## Test Cases Implemented

| No. | Feature | Scenario | Expected Result |
| --- | --- | --- | --- |
| 1 | Authentication | Login with standard user | Inventory page is displayed. |
| 2 | Authentication | Login with invalid username | Login is rejected with the correct message. |
| 3 | Authentication | Login with invalid password | Login is rejected with the correct message. |
| 4 | Authentication | Login without username | Username-required message is displayed. |
| 5 | Authentication | Login without password | Password-required message is displayed. |
| 6 | Authentication | Login with locked user | Locked-user message is displayed. |
| 7 | Inventory and Cart | Sort prices low to high | Product prices are in ascending order. |
| 8 | Inventory and Cart | Retain selected products | Cart contents remain after continuing shopping. |
| 9 | Inventory and Cart | Remove a product | Cart badge changes from one item to zero. |
| 10 | Checkout | Complete Week 6 purchase journey | Order is completed successfully. |
| 11 | Checkout | Submit without first name | First-name error is displayed. |
| 12 | Checkout | Submit without last name | Last-name error is displayed. |
| 13 | Checkout | Submit without postal code | Postal-code error is displayed. |

## Configuration Values

Configuration is stored in `appsettings.json`.

| Setting | Purpose |
| --- | --- |
| `baseUrl` | SauceDemo application URL. |
| `browser` | Default browser when `BROWSER` is not set. |
| `testIdAttribute` | Attribute used by Playwright `GetByTestId`. |
| `headless` | Controls visible or headless execution. |
| `timeoutMilliseconds` | Default Playwright action and navigation timeout. |
| `viewportWidth` | Browser viewport width. |
| `viewportHeight` | Browser viewport height. |
| `traceEnabled` | Enables Playwright trace capture. |
| `videoEnabled` | Enables browser-context video recording. |
| `screenshotOnFailure` | Enables screenshots for failed scenarios. |
| `evidenceDirectory` | Evidence folder below the .NET 10 test-output directory. |

`TestSettings` validates the URL, browser, timeout, viewport, test-id attribute, and evidence directory before browser execution.

## C# and Testing Concepts Used

- Classes, records, objects, properties, and methods
- Static classes and constants
- Lists and dictionaries
- Async and await
- Loops and conditional statements
- JSON deserialization and validation
- Environment variables
- Dependency injection through Reqnroll
- Page Object Model
- Reusable flows
- Scenario-specific state
- Separation of responsibilities
- DRY principle
- Gherkin and BDD
- Reqnroll bindings and hooks
- NUnit assertions, categories, and parallel execution
- Playwright locators, assertions, contexts, traces, and videos

## How To Run on Windows

### Prerequisites

- Windows 10 or Windows 11
- .NET 10 SDK
- Visual Studio or Visual Studio Code
- PowerShell

Open Windows PowerShell in the `Assignment8\SauceDemoBDD` folder.

Confirm that .NET 10 is installed:

```powershell
dotnet --list-sdks
```

Restore packages:

```powershell
dotnet restore .\SauceDemoBDD.csproj
```

Build the project and generate the NUnit tests from the Gherkin feature files:

```powershell
dotnet build .\SauceDemoBDD.csproj
```

Install the Playwright browsers after the first build:

```powershell
powershell -ExecutionPolicy Bypass -File .\bin\Debug\net10.0\playwright.ps1 install chromium webkit
```

Run all 13 BDD scenarios in Chromium:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\SauceDemoBDD.csproj --filter "TestCategory=bdd"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run all 13 BDD scenarios in WebKit:

```powershell
$env:BROWSER = "WebKit"
dotnet test .\SauceDemoBDD.csproj --filter "TestCategory=bdd"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

Run the BDD scenarios with detailed terminal output:

```powershell
$env:BROWSER = "Chromium"
dotnet test .\SauceDemoBDD.csproj --filter "TestCategory=bdd" --logger "console;verbosity=detailed"
Remove-Item Env:\BROWSER -ErrorAction SilentlyContinue
```

## Expected Terminal Output

Reqnroll should generate NUnit tests from the feature files, and NUnit should discover and execute 13 scenarios.

```text
Passed! - Failed: 0, Passed: 13, Skipped: 0, Total: 13
```

The scenario names and Given, When, and Then results are displayed in the detailed test output.

## Generated Evidence

| Output | Windows Location |
| --- | --- |
| Traces | `.\bin\Debug\net10.0\TestResults` |
| Videos | `.\bin\Debug\net10.0\TestResults` |
| Failure screenshots | `.\bin\Debug\net10.0\TestResults` |
| Allure raw results | `.\bin\Debug\net10.0\Reports\allure-results` |

## Expected Output

Gherkin scenarios clearly describe SauceDemo behavior.

Reqnroll converts the scenarios into executable NUnit tests.

Valid users can log in, while invalid, incomplete, and locked-user credentials are rejected.

Products can be sorted, added, retained, removed, and validated in the cart.

The complete Week 6 checkout journey succeeds.

Incomplete checkout information displays the correct error.

All 13 scenarios can run in Chromium or WebKit without changing source code.

## Conclusion

This project demonstrates Behavior-Driven Development using Gherkin, Reqnroll, NUnit, Playwright, C#, and .NET 10.

It completes Assignment 2 by applying the Page Object Model and BDD to the Week 6 SauceDemo scenario while keeping feature descriptions, step definitions, page objects, test data, configuration, browser lifecycle, and reports clearly separated.
