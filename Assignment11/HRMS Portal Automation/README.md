# HRINTIME Playwright Automation

C#/.NET 10 UI automation using Microsoft Playwright, NUnit, Reqnroll, and the Page Object Model.

## Project structure

- `Features/` contains the Gherkin scenarios.
- `StepDefinitions/` contains step bindings and every test assertion.
- `Pages/` contains UI locators, actions, and state queries only. Reusable page literals are private constants at the top of each class.
- `TestData/` contains expected business values and scenario inputs.
- `Configuration/` contains routes, browser engine types, settings loading, tags, and credential access.
- `Helper/` contains the browser lifecycle, launch/context factories, and execution limiter.
- `Hooks/TestHooks.cs` creates and disposes one isolated browser execution per scenario.
- `Context/ScenarioTestContext.cs` stores scenario-scoped page objects and results shared between steps.
- `Models/` contains result/request records passed between pages and steps.

`Support/` was removed because it only held scenario state; `Context/` now describes that responsibility directly.

## Browser concurrency

`appsettings.json` limits active browser executions:

```json
{
}
```


## Credentials

Set credentials as environment variables. Never store real credentials in source files or feature files.

```powershell
$env:HRMS_USERNAME = "your-username"
$env:HRMS_PASSWORD = "your-password"
```

Other supported environment overrides are documented in `.env.example`.

## Run

```powershell
dotnet restore .\HRIntimeAutomation.csproj
dotnet test .\HRIntimeAutomation.csproj
```

Use `HEADLESS=true` when browser windows should not be displayed.

### Sequential and parallel execution

```powershell
dotnet test .\HRIntimeAutomation.csproj

dotnet test .\HRIntimeAutomation.csproj
```


## Test categories

Reqnroll tags are exposed as NUnit test categories:

- `smoke` — the nine critical business flows: login, Dashboard, navigation, My Profile, Employee Directory, Leave Application, Attendance, Leave Correction, and logout.
- `regression` — the complete functional suite.
- `readOnly` — scenarios that do not create HRMS business records.
- `dataMutation` — leave application and leave correction scenarios that create records.
- `negative` — negative validation scenarios such as invalid login.

```powershell
# Critical smoke suite
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=smoke"

# Complete regression suite
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression"

# Regression without record-creating scenarios
dotnet test .\HRIntimeAutomation.csproj --filter "TestCategory=regression&TestCategory=readOnly"
```

## Cross-browser execution

The runner executes the selected suite once for Chromium, once for WebKit, or once for each when `All` is selected. Each run sets `BROWSER`, which is consumed by `BrowserFactory`.

```powershell
```

## Failure investigation and reporting

When a scenario fails, `TestHooks` asks `BrowserDriver` to capture a full-page PNG and a Playwright trace ZIP before closing the browser. Both files are stored under `TestArtifacts/` and attached to the NUnit result. Run the suite with the TRX logger for CI evidence:

```powershell
dotnet test .\HRIntimeAutomation.csproj --logger "trx" --results-directory TestResults
```

The investigation flow is:

1. Open the failed test in the TRX or Allure report.
2. Inspect the attached failure screenshot.
3. Open the trace ZIP with Playwright Trace Viewer to inspect actions, DOM snapshots, console, and network activity.
4. Use the Jenkins archived `TestResults/`, `TestArtifacts/`, and Allure output for the complete CI record.

## Layer responsibilities

1. A `When` step calls a page action and stores its actual result in `ScenarioTestContext` when needed.
2. A `Then` step compares the actual result with values from `TestData` using NUnit or Playwright assertions.
3. Page classes do not contain NUnit or Playwright assertions.


