# HRINTIME Playwright Automation

## Overview

**HRINTIME Playwright Automation** is a maintainable UI test automation framework for the HRINTIME HRMS Portal.

The framework is built using **C#/.NET 10**, **Microsoft Playwright**, **NUnit**, **Reqnroll (BDD)**, and the **Page Object Model (POM)**.

It provides business-readable BDD scenarios, reusable page objects, scenario isolation, controlled browser concurrency, secure credential handling, test categorization, CI/CD execution through Jenkins, Allure reporting, and failure investigation artifacts.

---

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Framework Architecture](#framework-architecture)
- [Project Structure](#project-structure)
- [Layer Responsibilities](#layer-responsibilities)
- [Prerequisites](#prerequisites)
- [Initial Setup](#initial-setup)
- [Configuration](#configuration)
- [Credentials and Environment Variables](#credentials-and-environment-variables)
- [Installing Playwright Browsers](#installing-playwright-browsers)
- [Running the Tests](#running-the-tests)
- [Test Categories](#test-categories)
- [Browser Configuration](#browser-configuration)
- [Parallel and Sequential Execution](#parallel-and-sequential-execution)
- [Test Results and Artifacts](#test-results-and-artifacts)
- [Allure Reporting](#allure-reporting)
- [Jenkins CI/CD](#jenkins-cicd)
- [Failed Test Retry](#failed-test-retry)
- [Failure Investigation](#failure-investigation)
- [Test Design Guidelines](#test-design-guidelines)
- [Hardcoding and Test Data Management](#hardcoding-and-test-data-management)
- [Security Guidelines](#security-guidelines)
- [Execution Flow](#execution-flow)
- [Troubleshooting](#troubleshooting)
- [Project Goals](#project-goals)

---

# Technology Stack

| Technology | Purpose |
|---|---|
| C# | Programming language |
| .NET 10 | Runtime and test project framework |
| Microsoft Playwright | Browser automation |
| NUnit | Test execution and assertions |
| Reqnroll | BDD and Gherkin support |
| Page Object Model | UI abstraction and maintainability |
| JSON | Framework configuration |
| Environment Variables | Credentials and environment-specific configuration |
| Allure | Test reporting |
| Jenkins | CI/CD execution |

---

# Framework Architecture

The framework separates business scenarios from UI implementation and framework infrastructure.

```text
                         ┌──────────────────────┐
                         │     Feature Files    │
                         │       Gherkin        │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │   Step Definitions   │
                         │  Scenario Execution  │
                         │     Assertions       │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │ ScenarioTestContext  │
                         │   Scenario State     │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │     Page Objects     │
                         │ Locators + Actions   │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │ Microsoft Playwright │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │    HRINTIME Portal   │
                         └──────────────────────┘
```

Supporting components:

```text
Configuration ──► Routes / Settings / Tags / Credentials
TestData ───────► Scenario Inputs / Expected Values
Models ─────────► Structured Data
Helper ─────────► Browser Lifecycle / Execution Limiting
Hooks ──────────► Scenario Setup / Cleanup
```

---

# Project Structure

```text
HRINTIME Automation
│
├── Features/
│   └── *.feature
│
├── StepDefinitions/
│   └── *.cs
│
├── Pages/
│   └── *.cs
│
├── TestData/
│   └── *.cs
│
├── Configuration/
│   └── *.cs
│
├── Helper/
│   └── *.cs
│
├── Hooks/
│   └── TestHooks.cs
│
├── Context/
│   └── ScenarioTestContext.cs
│
├── Models/
│   └── *.cs
│
├── Documentation/
│   └── HardcodingAudit.md
│
├── Reports/
│   └── allure-results/
│
├── TestResults/
│   └── *.trx
│
├── Screenshots/
│   └── failure screenshots
│
├── Traces/
│   └── Playwright traces
│
├── appsettings.json
├── .env.example
├── HRIntimeAutomation.csproj
├── Jenkinsfile
└── README.md
```

> `Support/` was removed because it previously contained scenario state. Scenario state is now represented explicitly by `Context/`.

---

# Layer Responsibilities

## `Features/`

Contains BDD feature files written in Gherkin.

Responsibilities:

- Describe business behavior.
- Define Given/When/Then scenarios.
- Apply scenario tags.
- Keep scenarios readable from a business perspective.

Feature files should describe **what the application should do**, not how Playwright performs the interaction.

Example:

```gherkin
Feature: Login

  @smoke
  Scenario: Login with valid credentials
    Given the user is on the HRINTIME login page
    When the user logs in with valid credentials
    Then the dashboard should be displayed
```

---

## `StepDefinitions/`

Contains Reqnroll step bindings.

Responsibilities:

- Bind Gherkin steps to executable code.
- Call appropriate page actions.
- Store actual results in `ScenarioTestContext` when required.
- Perform scenario-level assertions.

Step definitions should not duplicate large amounts of UI locator logic.

---

## `Pages/`

Contains Page Object classes.

Responsibilities:

- UI locators.
- Page actions.
- Page state queries.
- Reusable page-specific operations.

Page classes should contain only functionality related to the page they represent.

Reusable page literals are maintained as private constants at the top of the relevant class.

### Design Rule

Page classes do **not** contain NUnit or Playwright assertions.

Page objects expose actions and state. Test steps determine whether the observed state is correct.

---

## `TestData/`

Contains:

- Scenario inputs.
- Expected business values.
- Reusable test data.

This keeps test data separate from UI implementation and framework infrastructure.

---

## `Configuration/`

Contains framework configuration functionality, including:

- Application routes.
- Browser names.
- Settings loading.
- Test tags.
- Credential access.
- Environment-specific configuration.

---

## `Helper/`

Contains technical framework infrastructure, including:

- Browser lifecycle support.
- Browser/context factories.
- Browser execution limiting.
- Other reusable framework-level utilities.

Business-specific test logic should not be placed in this layer.

---

## `Hooks/TestHooks.cs`

Controls scenario-level setup and cleanup.

The hooks create and dispose an isolated browser execution for each scenario.

Conceptual lifecycle:

```text
Before Scenario
      │
      ▼
Acquire execution slot
      │
      ▼
Create browser/context/page
      │
      ▼
Execute scenario
      │
      ▼
Capture required artifacts
      │
      ▼
Dispose browser resources
      │
      ▼
Release execution slot
```

---

## `Context/ScenarioTestContext.cs`

Stores scenario-scoped state shared between steps.

Examples:

- Page object instances.
- Actual values captured during execution.
- Results required by subsequent steps.

The context is specifically responsible for **scenario state management**.

---

## `Models/`

Contains structured records/models passed between pages, steps, and supporting components.

Models should represent data and should not contain unrelated framework functionality.

---

# Prerequisites

The following are required for local execution:

- .NET 10 SDK.
- Git.
- Windows environment with the required browser support.
- Playwright browser binaries.
- Access to the HRINTIME test environment.
- Valid HRINTIME test credentials.

For CI/CD execution, Jenkins must be installed and configured on the build agent.

---

# Initial Setup

## 1. Clone the Repository

Clone the repository containing the automation project:

```powershell
git clone <repository-url>
```

Navigate to the project:

```powershell
cd "Assignment11\HRMS Portal Automation"
```

---

## 2. Restore Dependencies

```powershell
dotnet restore .\HRIntimeAutomation.csproj
```

---

## 3. Build the Project

```powershell
dotnet build .\HRIntimeAutomation.csproj
```

A successful build confirms that the project dependencies and source code compile correctly.

---

# Configuration

The primary framework configuration is maintained in:

```text
appsettings.json
```

Example browser concurrency configuration:

```json
{
  "parallelEnabled": true,
  "parallelWorkers": 3
}
```

| Setting | Description |
|---|---|
| `parallelEnabled` | Enables or disables parallel browser execution |
| `parallelWorkers` | Maximum number of active browser executions |

Additional environment overrides are documented in:

```text
.env.example
```

---

# Credentials and Environment Variables

Credentials must never be committed to source control.

Do not store real credentials in:

- C# source files.
- Feature files.
- Test data files.
- `appsettings.json`.
- README files.
- Git history.

Use environment variables.

## PowerShell

```powershell
$env:HRMS_USERNAME = "your-username"
$env:HRMS_PASSWORD = "your-password"
```

For headless execution:

```powershell
$env:HEADLESS = "true"
```

Other supported environment variables are documented in:

```text
.env.example
```

---

# Installing Playwright Browsers

After restoring the project, install the Playwright browser binaries required by the project.

For a standard .NET Playwright setup, the generated installation script can be executed from the build output.

Typical command:

```powershell
pwsh .\bin\Debug\net10.0\playwright.ps1 install
```

If PowerShell script execution is restricted by the machine policy, use the appropriate executable/script route permitted by the environment.

The required Playwright browser binaries must be available before executing the complete test suite.

---

# Running the Tests

## Restore

```powershell
dotnet restore .\HRIntimeAutomation.csproj
```

## Build

```powershell
dotnet build .\HRIntimeAutomation.csproj
```

## Run Complete Suite

```powershell
dotnet test .\HRIntimeAutomation.csproj
```

---

# Headless Execution

Set:

```powershell
$env:HEADLESS = "true"
```

Then run:

```powershell
dotnet test .\HRIntimeAutomation.csproj
```

Headless execution is recommended for CI/CD environments where browser windows should not be displayed.

---

# Test Categories

Reqnroll tags are exposed as NUnit test categories.

| Category | Purpose |
|---|---|
| `smoke` | Critical, non-mutating checks |
| `regression` | Complete functional regression suite |
| `readOnly` | Scenarios that do not create HRMS business records |
| `dataMutation` | Scenarios that create or modify HRMS records |
| `negative` | Negative validation scenarios |

---

## Smoke Tests

Smoke tests cover critical checks such as:

- Valid login.
- Dashboard availability.
- Navigation.
- Logout.

Run:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=smoke"
```

---

## Regression Tests

Run the regression suite with:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression"
```

---

## Read-Only Tests

Run:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=readOnly"
```

These scenarios are intended not to create HRMS business records.

---

## Data Mutation Tests

Run:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=dataMutation"
```

These scenarios create or modify HRMS business records.

---

## Negative Tests

Run:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=negative"
```

These scenarios cover validation and negative behavior such as invalid login.

---

# Browser Configuration

The framework supports the configured Playwright browsers:

- Chromium.
- WebKit.

The Jenkins pipeline exposes the following browser options:

```text
Chromium
WebKit
All
```

When `All` is selected, the CI/CD pipeline executes the test suite against both configured browsers.

---

# Parallel and Sequential Execution

Browser execution is controlled through configuration.

Default configuration:

```json
{
  "parallelEnabled": true,
  "parallelWorkers": 3
}
```

When parallel execution is enabled, NUnit may schedule multiple scenarios, while `BrowserExecutionLimiter` controls the number of active browser executions.

With three workers:

```text
Scenario 1 ──► Browser ──┐
Scenario 2 ──► Browser ──┼──► Maximum 3 active executions
Scenario 3 ──► Browser ──┘
Scenario 4 ──► Waiting
Scenario 5 ──► Waiting
```

## Disable Parallel Execution

```powershell
$env:PARALLEL_ENABLED = "false"
```

## Override Worker Count

```powershell
$env:PARALLEL_WORKERS = "5"
```

Worker count should be selected according to available machine resources and the target environment.

---

# Test Results and Artifacts

The framework can generate the following artifacts:

```text
TestResults/
Screenshots/
Traces/
Reports/
```

## TRX Results

.NET test execution generates TRX files containing structured test execution information.

Typical location:

```text
TestResults/*.trx
```

TRX files contain information such as:

- Test names.
- Test outcomes.
- Execution information.
- Failure information.

---

## Screenshots

Failure screenshots provide the visual state of the application at the time of failure.

Typical location:

```text
Screenshots/
```

---

## Playwright Traces

Playwright traces provide detailed browser execution information useful for diagnosing:

- Navigation problems.
- Locator failures.
- Timing issues.
- Browser state.
- Interaction failures.

Typical location:

```text
Traces/
```

---

# Allure Reporting

The project supports Allure reporting.

Allure result files are stored under:

```text
Reports/allure-results/
```

The Jenkins pipeline publishes the generated Allure results as a build report.

Allure can be used to analyze:

- Passed scenarios.
- Failed scenarios.
- Test steps.
- Execution duration.
- Failure information.
- Supporting execution details.

---

# Jenkins CI/CD

The project includes a Jenkins pipeline:

```text
Jenkinsfile
```

The pipeline supports:

- Git SCM checkout.
- SCM polling.
- Browser selection.
- Test category selection.
- Parallel/sequential execution selection.
- Retry count selection.
- .NET restore.
- .NET build.
- Automated test execution.
- Failed-test retry.
- Allure reporting.
- TRX/artifact archiving.
- Email notification.

---

## Jenkins Parameters

| Parameter | Options | Purpose |
|---|---|---|
| `EXECUTION_MODE` | `Parallel`, `Sequential` | Selects execution mode |
| `BROWSER` | `Chromium`, `WebKit`, `All` | Selects browser coverage |
| `TEST_CATEGORY` | `All`, `smoke`, `regression` | Selects test scope |
| `RETRY_COUNT` | `0`, `1`, `2`, `3` | Controls retry attempts |

---

## Jenkins Pipeline Flow

```text
Git Repository
      │
      ▼
SCM Polling
      │
      ▼
Checkout Assignment Branch
      │
      ▼
Prepare
      │
      ▼
Restore
      │
      ▼
Build
      │
      ▼
Run Tests
      │
      ▼
Retry Failed Tests
      │
      ▼
Allure Report
      │
      ▼
Archive Results
      │
      ▼
Email Notification
```

---

# Jenkins SCM Polling

The pipeline uses Jenkins SCM polling to detect repository changes.

Configured polling schedule:

```text
H/2 * * * *
```

The Jenkins job should be configured to monitor the intended assignment branch and use the project's Jenkinsfile path.

---

# Jenkins Email Notification

The pipeline sends a notification using the standard Jenkins `mail()` step.

The email includes:

- Build number.
- Build status.
- Build duration.
- Total tests.
- Passed tests.
- Failed tests.
- Browser.
- Execution mode.
- Test category.
- Retry count.
- Allure report URL.
- Jenkins build URL.

Example:

```text
HRMS Portal Automation - Jenkins Build

BUILD INFORMATION

Build Number   : 25
Build Status   : SUCCESS
Build Duration : 2 min 14 sec

TEST SUMMARY

Total Tests    : 28
Passed Tests   : 27
Failed Tests   : 1

EXECUTION DETAILS

Browser        : Chromium
Execution Mode : Parallel
Test Category  : regression
Retry Count    : 2

REPORTS

Detailed Allure Report:
<jenkins-build-url>/allure/

Jenkins Build:
<jenkins-build-url>
```

---

# Failed Test Retry

The Jenkins pipeline supports retrying failed tests.

The retry count is controlled through:

```text
RETRY_COUNT
```

Available values:

```text
0
1
2
3
```

## Retry Flow

```text
Initial Test Execution
        │
        ▼
Identify Failed Tests
        │
        ├── No failures ──► Continue
        │
        ▼
Retry Failed Tests
        │
        ▼
Failures Remaining?
        │
        ├── Yes ──► Retry until configured limit
        │
        └── No ───► Continue
```

Retry results are stored under the configured `TestResults` directory.

---

# Failure Investigation

When a test fails, use the following investigation flow.

## 1. Review TRX Results

Check:

```text
TestResults/
```

Review:

- Test name.
- Outcome.
- Failure information.
- Execution information.

## 2. Review Screenshot

Check:

```text
Screenshots/
```

Use the screenshot to determine the UI state at failure time.

## 3. Review Playwright Trace

Check:

```text
Traces/
```

Use the trace to investigate browser actions, navigation, locators, and timing.

## 4. Review Allure

Use the Jenkins Allure report to review the complete execution and failure details.

---

# Test Design Guidelines

## Feature Files

Feature files should remain business-readable.

Preferred:

```gherkin
When the user submits the leave application
Then the leave application should be submitted successfully
```

Avoid exposing implementation details:

```gherkin
When the user clicks locator "#submitButton"
```

---

## Step Definitions

Step definitions should:

- Bind Gherkin steps.
- Call page actions.
- Store required actual results.
- Perform scenario validation.

They should not contain duplicated UI locator implementations.

---

## Page Objects

Page objects should:

- Own page-specific locators.
- Perform page-specific actions.
- Expose useful state queries.
- Hide UI implementation details from feature files.

Page objects should not contain test assertions.

---

## Test Data

Expected business values and scenario inputs should be maintained in the appropriate `TestData` layer rather than duplicated throughout step definitions.

---

# Hardcoding and Test Data Management

The project includes a project-wide hardcoding review:

```text
Documentation/HardcodingAudit.md
```

The intended value ownership is:

| Value Type | Location |
|---|---|
| UI locators | `Pages/` |
| Application routes | `Configuration/` |
| Browser settings | `Configuration/` / environment |
| Credentials | Environment variables |
| Expected business values | `TestData/` |
| Scenario inputs | `TestData/` / feature data |
| Scenario state | `Context/` |
| Structured data | `Models/` |
| Browser infrastructure | `Helper/` |

The goal is to keep each value in its appropriate layer and avoid unnecessary duplication.

---

# Security Guidelines

## Never Commit Credentials

Never commit:

```text
Usernames
Passwords
API keys
Access tokens
Secrets
```

Use environment variables instead.

## `.env.example`

The example environment file should contain placeholders only.

Example:

```text
HRMS_USERNAME=your-username
HRMS_PASSWORD=your-password
HEADLESS=true
```

Never replace these placeholders with real credentials before committing.

---

# Execution Flow

The complete local test execution flow is:

```text
                  ┌─────────────────┐
                  │  Feature File   │
                  │    Gherkin      │
                  └────────┬────────┘
                           │
                           ▼
                  ┌─────────────────┐
                  │ Step Definitions│
                  └────────┬────────┘
                           │
                           ▼
                  ┌─────────────────┐
                  │ Scenario Context│
                  └────────┬────────┘
                           │
                           ▼
                  ┌─────────────────┐
                  │   Page Objects  │
                  └────────┬────────┘
                           │
                           ▼
                  ┌─────────────────┐
                  │   Playwright    │
                  └────────┬────────┘
                           │
                           ▼
                  ┌─────────────────┐
                  │ HRINTIME Portal │
                  └─────────────────┘
```

Supporting framework components:

```text
Configuration ──► Runtime settings
TestData ───────► Inputs / expected values
Models ─────────► Structured data
Helper ─────────► Browser infrastructure
Hooks ──────────► Scenario lifecycle
```

---

# Troubleshooting

## `.NET` Command Not Found

Check the installed SDK:

```powershell
dotnet --version
```

The required .NET SDK must be installed and available on the system PATH.

---

## Restore Fails

Run:

```powershell
dotnet restore .\HRIntimeAutomation.csproj
```

Review the package and dependency errors reported by the .NET CLI.

---

## Build Fails

Run:

```powershell
dotnet build .\HRIntimeAutomation.csproj
```

Resolve compilation errors before executing the tests.

---

## Playwright Browser Is Not Installed

Install the required Playwright browser binaries using the Playwright installation mechanism generated by the project's `Microsoft.Playwright` package.

---

## Browser Does Not Start in CI

Check:

1. Playwright browser binaries are installed.
2. The selected browser is supported.
3. `HEADLESS=true` is configured when required.
4. Jenkins has access to the required environment variables.
5. The build agent has sufficient resources.

---

## Credentials Are Missing

Verify the environment variables exist in the execution environment:

```powershell
$env:HRMS_USERNAME
$env:HRMS_PASSWORD
```

Do not print real credential values to the Jenkins console.

---

## Parallel Execution Problems

Temporarily disable parallel execution:

```powershell
$env:PARALLEL_ENABLED = "false"
```

Or reduce the worker count:

```powershell
$env:PARALLEL_WORKERS = "1"
```

This can help determine whether a failure is related to concurrency or environment resources.

---

# Project Goals

The framework is designed to provide:

### Maintainability

Clear separation of responsibilities makes the automation framework easier to modify and extend.

### Reusability

Page actions, configuration, test data, and framework utilities can be reused across scenarios.

### Readability

BDD feature files express scenarios in business-readable language.

### Reliability

Scenario isolation and controlled browser concurrency reduce unnecessary test interference.

### Security

Credentials remain outside source-controlled files.

### Scalability

A modular architecture allows additional HRMS functionality to be automated without unnecessarily coupling existing components.

### CI/CD Readiness

The project supports command-line execution and Jenkins integration for automated execution, retry, reporting, artifact archiving, and email notification.

---

# Quick Start

For a new environment:

```powershell
# Restore dependencies
dotnet restore .\HRIntimeAutomation.csproj

# Build the project
dotnet build .\HRIntimeAutomation.csproj

# Configure credentials
$env:HRMS_USERNAME = "your-username"
$env:HRMS_PASSWORD = "your-password"

# Optional: run headless
$env:HEADLESS = "true"

# Run the complete suite
dotnet test .\HRIntimeAutomation.csproj
```

Smoke suite:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=smoke"
```

Regression suite:

```powershell
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression"
```

---

# Repository Documentation

Important supporting files:

```text
README.md
Jenkinsfile
appsettings.json
.env.example
Documentation/HardcodingAudit.md
HRIntimeAutomation.csproj
```

---

# Summary

**HRINTIME Playwright Automation** combines:

```text
C# / .NET 10
       +
Microsoft Playwright
       +
NUnit
       +
Reqnroll / BDD
       +
Page Object Model
       +
Scenario Isolation
       +
Controlled Browser Concurrency
       +
Secure Environment Configuration
       +
Allure Reporting
       +
Jenkins CI/CD
```

The framework is structured to keep business behavior, UI automation, test data, configuration, scenario state, and infrastructure responsibilities clearly separated while supporting reliable local and CI/CD test execution.
