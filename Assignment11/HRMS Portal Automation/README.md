# HRINTIME Playwright Automation Framework

## 1. Project Overview

The **HRINTIME Playwright Automation Framework** is a UI automation testing project developed for the HRINTIME Human Resource Management System.

The framework is built using:

- **C#**
- **.NET 10**
- **Microsoft Playwright**
- **NUnit**
- **Reqnroll**
- **Page Object Model (POM)**
- **Gherkin/BDD**
- **Allure reporting**
- **Playwright screenshots and traces**
- **Jenkins CI/CD pipeline integration**

The purpose of the project is not simply to automate browser clicks. It provides a structured automated regression framework capable of validating important HRMS business functionality such as authentication, employee information, navigation, leave management, attendance, employee-directory operations, logout, and session protection.

The framework separates test scenarios, test data, UI interaction logic, assertions, configuration, browser management, and reporting. This makes the automation easier to maintain as the HRINTIME application evolves.

## 2. Main Objectives

The main objectives of this automation project are to:

1. Automate important HRINTIME business workflows.
2. Reduce repetitive manual regression testing.
3. Validate both positive and negative application behavior.
4. Verify important HRMS business rules.
5. Keep test code maintainable through Page Object Model.
6. Make scenarios understandable through BDD/Gherkin.
7. Execute independent tests in parallel while controlling browser concurrency.
8. Keep browser sessions isolated between scenarios.
9. Generate useful debugging evidence when failures occur.
10. Produce test results suitable for local execution and CI environments.
11. Integrate automated regression execution into a **Jenkins pipeline**.
12. Support smoke, regression, read-only, mutation, and negative test categorization.

## 3. Technology Stack

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| .NET 10 | Runtime and project platform |
| Microsoft Playwright | Browser/UI automation |
| NUnit | Test runner and assertion framework |
| Reqnroll | BDD implementation for Gherkin scenarios |
| Gherkin | Human-readable feature/scenario definitions |
| Page Object Model | Separates UI interaction from test logic |
| Allure | Rich test execution reporting |
| TRX | Standard .NET test result format |
| Playwright Trace Viewer | Investigation of failed browser executions |
| Jenkins | Continuous Integration execution and reporting |
| JSON configuration | Framework/application configuration |
| Environment variables / `.env` | Credential and environment configuration |

## 4. Architecture of the Framework

The framework follows a layered design.

```text
Feature File
     ↓
Step Definition
     ↓
Page Object
     ↓
Microsoft Playwright
     ↓
HRINTIME Web Application
```

Test data and configuration are provided separately:

```text
TestData ───────────────→ Step Definitions
Configuration ─────────→ Browser / Environment / Routes
```

Results can be stored in the scenario context:

```text
Page Object
     ↓
Actual Result
     ↓
ScenarioTestContext
     ↓
Then Step
     ↓
Assertion
```

This separation is important because a Page Object should know **how to interact with a page**, while the Step Definition should decide **whether the observed result is correct**.

## 5. Project Structure

### `Features/`
Contains BDD feature files written using Gherkin. These describe application behavior through `Feature`, `Scenario`, `Given`, `When`, and `Then` statements.

### `StepDefinitions/`
Contains the C# implementation of Gherkin steps. Step Definitions call Page Object actions, retrieve application state, store results in `ScenarioTestContext`, obtain expected values from `TestData`, and perform NUnit/Playwright assertions.

### `Pages/`
Contains Page Object classes representing different parts of HRINTIME. Page Objects locate UI elements, click controls, enter values, select dropdown options, navigate components, read displayed information, and return application state/results. Test assertions should remain outside this layer.

### `TestData/`
Contains expected business values and scenario-specific inputs, keeping test data separate from UI implementation and assertions.

### `Configuration/`
Manages application routes, browser engine types, environment/configuration loading, credentials, tags, and test settings.

### `Utilities/`
Contains reusable browser/framework infrastructure such as browser lifecycle and launch/context factories.

### `Hooks/TestHooks.cs`
Executes framework setup and teardown around BDD scenarios, including browser/context creation and failure-artifact capture.

### `Context/ScenarioTestContext.cs`
Provides scenario-scoped storage for page objects and actual results shared between steps.

### `Models/`
Contains structured result/request records passed between pages and step definitions.

## 6. Page Object Model

The Page Object Model separates UI implementation details from business-level test logic. For example, `LeaveApplicationPage` knows how to open Apply Leave, select Casual Leave, choose dates, mark a half-day, enter a description, submit the request, and return the application's response. The Step Definition determines whether that response satisfies the expected requirement.

This improves maintainability because a locator change can usually be corrected in one Page Object instead of many scenarios.

## 7. Credentials and `.env`

Credentials should never be hardcoded in feature files, Page Objects, Step Definitions, or source control.

A local `.env` can contain:

```text
HRMS_USERNAME=your-username
HRMS_PASSWORD=your-password
```

The actual `.env` containing real credentials should not be committed. `.env.example` can document the required variable names.

Credentials can also be supplied through PowerShell:

```powershell
$env:HRMS_USERNAME = "your-username"
$env:HRMS_PASSWORD = "your-password"
```

This also allows Jenkins to inject credentials securely.

## 8. Browser Management and Scenario Isolation

The browser infrastructure selects the browser engine, launches Playwright, creates isolated browser contexts, configures execution, and disposes resources correctly. Each scenario receives an isolated execution environment rather than depending on browser state created by another scenario.

This avoids test dependencies such as “Test B only works if Test A ran first.”

## 9. Test Coverage

The automation suite covers the major HRINTIME areas required by the assignment.

| Area | Coverage |
|---|---|
| Authentication | Valid and invalid login combinations |
| Dashboard | Current-date/calendar validation |
| Navigation | Main HRMS navigation and navigation stability |
| My Profile | Personal, job and work-scheme information |
| Resignation | UI validation and date-calculation rules |
| Leave Application | Positive and mandatory-field scenarios |
| Leave Correction | Work-from-home, half-day and validation scenarios |
| Attendance | Date ranges, weekly-off and boundary validation |
| Employee Directory | Job-title filtering and pagination |
| Footer | Social-media links |
| Logout | Successful logout |
| Session Security | Protection after logout |
| Framework diagnostics | Screenshots/traces for failed executions |

The suite includes positive testing, negative testing, field validation, boundary testing, business-rule validation, navigation testing, filtering, pagination, authentication, session protection, and failure diagnostics.

## 10. Detailed Test Cases

The current suite contains approximately **34 NUnit executions**, with the majority representing HRINTIME BDD scenarios/example cases and an additional framework-oriented failure-artifact validation depending on the project version.

### TC01 — Successful Login
Opens HRINTIME, enters valid credentials, submits the login request, and verifies that Dashboard becomes available.

### TC02 — Valid Username + Invalid Password
Verifies that a valid username cannot authenticate with an invalid password.

### TC03 — Invalid Username + Valid Password
Verifies that a valid password cannot authenticate an invalid username.

### TC04 — Invalid Username + Invalid Password
Verifies rejection of completely incorrect credentials.

### TC05 — Empty Username
Verifies required-field behavior when username/email is absent.

### TC06 — Empty Password
Verifies required-field behavior when password is absent.

### TC07 — Empty Username and Password
Verifies that completely empty credentials cannot authenticate.

### TC08 — Username With Additional Spaces
Tests how authentication handles whitespace added to the username.

### TC09 — Password With Additional Spaces
Tests how authentication handles whitespace added to the password.

### TC10 — Dashboard Current Calendar Date
Validates the current day, month, year, and current-date indication dynamically rather than using a permanently hardcoded date.

### TC11 — Required HRINTIME Navigation
Exercises required sidebar destinations such as Dashboard, Organization, My Profile, Employee Directory, Attendance Record, Leaves Application, and Leave Correction, confirming that the expected pages open.

### TC12 — Navigation After Refresh and Module Toggling
Navigates to Employee Directory, refreshes the page, collapses/reopens modules, and verifies navigation remains usable.

### TC13 — Personal, Job and Work Scheme Information
Validates configured employee personal details, Job Title, Department, Employment Status, joining dates, and work-scheme information.

### TC14 — HRINTIME Last Working Date
Reads Date of Apply and Last Working Date from HRINTIME and validates the displayed last-working date against the expected notice-period rule.

The calculation is conceptually:

```csharp
expectedLastWorkingDate = dateOfApply.AddMonths(2).AddDays(-1);
```

### TC15 — Standard Resignation Date
Validates the resignation-date calculation with a normal mid-month date.

### TC16 — End-of-Month Resignation Date
Validates calculation behavior around dates such as January 31.

### TC17 — Year-Boundary Resignation Calculation
Validates a resignation calculation that crosses into the following year.

### TC18 — Leap-Year Calculation
Validates calculation behavior involving leap day/February.

### TC19 — Successful Casual Leave Application
Navigates to Leaves Application, selects Casual Leave, dynamically chooses an appropriate future range containing a weekend without beginning or ending on the weekend, marks the required date as half-day, enters a unique description, submits, and validates the application response.

### TC20 — Leave Without Leave Type
Negative validation test confirming an incomplete leave request cannot be accepted without Leave Type.

### TC21 — Leave Without Date
Negative validation test confirming the Date requirement is enforced.

### TC22 — Work From Home Correction With Half-Day
Opens Leave Correction, selects Work from home, chooses a two-day range, marks one date as half-day, verifies the half-day state, enters a unique description, submits the correction, and validates the resulting information. A two-day correction with one half-day corresponds to 1.5 days.

### TC23 — Correction Without Correction Type
Negative test for missing Correction Type.

### TC24 — Correction Without Date
Negative test for missing date/range.

### TC25 — Attendance and Weekly Off
Selects a range containing weekend days, validates attendance records, applies Weekly Off filtering, and verifies weekend/weekly-off behavior.

### TC26 — One-Day Attendance Boundary
Ensures records returned for a one-day filter remain inside the requested date.

### TC27 — Two-Day Attendance Boundary
Ensures records returned for a two-day range remain inside the selected interval.

### TC28 — Four-Day Attendance Boundary
Ensures a four-day filter does not leak records from outside the requested range.

### TC29 — Director of Engineering Filter
Navigates to Employee Directory, selects `Director of Engineering`, switches to Table View, and validates expected filtered employee records such as Archit Jain, Kapil Paliwal, and Yatin Yogi.

### TC30 — Employee Directory Pagination
Verifies no page contains more than 12 employee records, current-page indication is correct, Previous/Next controls behave correctly, and first/intermediate/last-page boundaries work dynamically.

### TC31 — Social Media Footer Links
Validates HRINTIME footer destinations for YouTube, Facebook, LinkedIn, and X/Twitter. The focus is validating the application's link configuration rather than treating external social-site anti-bot behavior as an HRINTIME defect.

### TC32 — Logout Functionality
From an authenticated session, opens the profile menu, selects Logout, and verifies that the Login page is displayed.

### TC33 — Protected Pages After Logout
After logout, attempts direct Dashboard access, refresh, and browser-back navigation, verifying that authentication remains required and protected content is not restored.

### TC34 — Failure Artifacts
Where retained, `FailureArtifactsTest.cs` validates the automation framework rather than HRINTIME business functionality. It verifies screenshot and Playwright trace generation for failed executions.

## 11. Test Categories

Reqnroll tags are exposed as NUnit categories.

- **`smoke`** — critical business workflows such as Login, Dashboard, Navigation, My Profile, Employee Directory, Leave Application, Attendance, Leave Correction, and Logout.
- **`regression`** — broader functional suite.
- **`readOnly`** — scenarios that do not intentionally create HRMS business records.
- **`dataMutation`** — scenarios such as Leave Application and Leave Correction that create or modify application data.
- **`negative`** — invalid input and mandatory-field scenarios.

Run smoke tests:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=smoke"
```

Run regression:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression"
```

Run read-only regression:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression&TestCategory=readOnly"
```

## 12. Running the Project

### Restore dependencies

```powershell
dotnet restore .\HRIntimeAutomation.csproj
```

### Build

```powershell
dotnet build .\HRIntimeAutomation.csproj
```

### Install Playwright browsers on a new environment

```powershell
pwsh .\bin\Debug\net10.0\playwright.ps1 install
```

### Run all tests

```powershell
dotnet test .\HRIntimeAutomation.csproj
```

### Run headless

```powershell
$env:HEADLESS = "true"
dotnet test .\HRIntimeAutomation.csproj
```

## 13. Controlled Parallel Execution

The framework limits concurrent browser execution so the complete suite does not open every browser window at once. This balances execution speed with developer-machine/Jenkins-agent resource usage and reduces pressure on the HRINTIME test environment.

State-changing tests require particular care because simultaneous leave/correction operations against the same account can interfere with one another.

## 14. Failure Investigation and Reporting

When a scenario fails, the framework can capture:

- full-page screenshots;
- Playwright trace ZIP files;
- NUnit results;
- Allure results.

Screenshots show the visible application state at failure. Playwright traces provide deeper information such as actions, DOM snapshots, timing, console information, and network activity.

### Generate TRX results

```powershell
dotnet test .\HRIntimeAutomation.csproj --logger "trx" --results-directory TestResults
```

## 15. Allure Reporting

After running tests, verify that Allure result files exist:

```powershell
Get-ChildItem .\Reports\allure-results
```

Serve the report on Windows:

```powershell
allure.cmd serve .\Reports\allure-results
```

Generate a persistent report:

```powershell
allure.cmd generate .\Reports\allure-results --clean -o .\Reports\allure-report
```

Open it:

```powershell
allure.cmd open .\Reports\allure-report
```

Using `allure.cmd` avoids PowerShell execution-policy problems that can block the npm-generated `allure.ps1` wrapper.

## 16. Jenkins Pipeline Integration

The HRINTIME automation project is integrated with a **Jenkins pipeline**, allowing the automation suite to participate in Continuous Integration rather than being limited to manual local execution.

A typical Jenkins flow is:

```text
Developer pushes code
        ↓
Jenkins pipeline starts
        ↓
Checkout source code
        ↓
Restore .NET dependencies
        ↓
Build automation project
        ↓
Prepare Playwright browsers
        ↓
Inject credentials/configuration
        ↓
Execute smoke/regression tests
        ↓
Generate TRX and Allure results
        ↓
Collect screenshots and traces
        ↓
Publish/archive CI artifacts
        ↓
Display build and test status
```

### Why Jenkins integration matters

Jenkins provides repeatable automated execution. Tests can be triggered after code changes, deployments, on demand, or according to a schedule. This reduces dependence on a developer manually running the suite.

### Jenkins credentials

Sensitive values such as `HRMS_USERNAME` and `HRMS_PASSWORD` should be supplied through Jenkins credentials/environment variables rather than committed to Git.

### Jenkins evidence

Useful pipeline artifacts include:

```text
TestResults/
TestArtifacts/
Reports/allure-results/
Reports/allure-report/
```

These can provide pass/fail evidence, screenshots, traces, Allure results, and execution history.

### Representative Jenkins test command

```powershell
dotnet test .\HRIntimeAutomation.csproj --logger "trx" --results-directory TestResults
```

For a smoke stage:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=smoke"
```

For a regression stage:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression"
```

## 17. Locator Strategy

The framework should prefer stable user-facing selectors such as:

```csharp
GetByRole(...)
GetByLabel(...)
GetByPlaceholder(...)
GetByText(...)
```

rather than generated Mantine class names. Semantic locators generally represent what the user actually interacts with and are more maintainable across UI builds.

## 18. Dynamic Test Data

Several scenarios avoid permanently hardcoded dates. Dynamic date logic is important for Dashboard current-date validation, Leave Application, Leave Correction, Attendance, and resignation calculations.

State-changing scenarios can also use unique descriptions such as:

```text
ANSHTEST_<timestamp>
Leave correction test ansh_<timestamp>
```

This makes automation-created records easier to distinguish.

## 19. Why BDD, NUnit, Playwright, and POM Are Used

### BDD / Reqnroll
Provides readable Given/When/Then scenarios and improves traceability between business requirements and automation.

### NUnit
Provides test discovery, execution, assertions, categories, result reporting, and `dotnet test` integration.

### Microsoft Playwright
Provides modern browser automation, semantic locators, auto-waiting, browser contexts, screenshots, traces, popup handling, and browser navigation.

### Page Object Model
Keeps UI implementation details concentrated in Page classes so test scenarios remain readable and locator changes are easier to maintain.

## 20. Complete Testing Strategy

| Technique | Example |
|---|---|
| Positive testing | Valid Login |
| Negative testing | Invalid credentials |
| Mandatory-field testing | Leave without date |
| Boundary testing | Jan 31 resignation |
| Calendar testing | Leap day |
| UI testing | Dashboard |
| Navigation testing | Sidebar |
| Data validation | My Profile |
| Filtering | Employee Directory |
| Pagination | Employee Directory pages |
| Business-rule testing | Resignation calculation |
| State-changing testing | Leave Application |
| Session/security testing | Protected Dashboard after logout |
| External-link validation | Footer |
| Framework diagnostics | Screenshot/trace generation |
| CI testing | Jenkins pipeline |

## 21. Limitations and Practical Considerations

- External websites such as Facebook, LinkedIn, YouTube, or X can block automated browsers. HRINTIME should primarily be responsible for exposing the correct external destination.
- Shared test accounts can cause state-changing tests to interfere with one another.
- Attendance, leave balances, and employee records can change over time.
- Test-environment/server failures can cause automation failures even when the test code is correct.
- Major UI changes may require Page Object locator updates.

These limitations are why screenshots, traces, meaningful errors, test isolation, and Jenkins artifacts are important.

## 22. Typical Failure Investigation Workflow

```text
1. Read the failing test/scenario message
        ↓
2. Identify the failing step
        ↓
3. Check Allure/TRX result
        ↓
4. Inspect the failure screenshot
        ↓
5. Open the Playwright trace
        ↓
6. Determine whether it is:
   - Application defect
   - Test-data problem
   - Locator problem
   - Environment problem
        ↓
7. Correct the appropriate layer
```

## 23. Conclusion

The **HRINTIME Playwright Automation Framework** provides an organized automated-testing solution for major HRMS workflows using **C#, .NET 10, Microsoft Playwright, NUnit, Reqnroll, and Page Object Model**.

Its coverage extends across authentication, Dashboard behavior, navigation, employee information, resignation calculations, leave management, attendance, employee-directory filtering and pagination, footer links, logout, and post-logout session protection.

The framework includes both **positive and negative testing**, together with boundary and business-rule validation. Dynamic test data is used where appropriate so scenarios are less dependent on permanently hardcoded dates.

The architecture separates business scenarios, step bindings/assertions, Page Object interactions, test data/configuration, and browser infrastructure. This improves readability, maintainability, and scalability.

Failure handling through **screenshots and Playwright traces** provides useful diagnostic evidence. **TRX and Allure reporting** make execution results easier to analyze and retain.

Finally, integration with a **Jenkins pipeline** enables Continuous Integration execution. Jenkins can run smoke or regression tests, inject credentials securely, execute browsers headlessly, collect test results, retain screenshots/traces, and publish reporting output.

Therefore, this project is not simply a collection of automated UI scripts. It is a structured **BDD-based UI test automation framework with controlled execution, business-level test coverage, diagnostics, reporting, and CI integration**, designed to provide repeatable validation of the HRINTIME application.
