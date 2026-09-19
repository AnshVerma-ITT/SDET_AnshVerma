# HRINTIME Playwright Automation

C#/.NET 10 UI automation using Microsoft Playwright, NUnit, Reqnroll, and the Page Object Model.

## Project structure

- `Features/` contains the Gherkin scenarios.
- `StepDefinitions/` contains step bindings and every test assertion.
- `Pages/` contains UI locators, actions, and state queries only. Reusable page literals are private constants at the top of each class.
- `TestData/` contains expected business values and scenario inputs.
- `Configuration/` contains routes, browser names, settings loading, tags, and credential access.
- `Helper/` contains the browser lifecycle, launch/context factories, and execution limiter.
- `Hooks/TestHooks.cs` creates and disposes one isolated browser execution per scenario.
- `Context/ScenarioTestContext.cs` stores scenario-scoped page objects and results shared between steps.
- `Models/` contains result/request records passed between pages and steps.

`Support/` was removed because it only held scenario state; `Context/` now describes that responsibility directly.

## Browser concurrency

`appsettings.json` limits active browser executions:

```json
{
  "parallelEnabled": true,
  "parallelWorkers": 3
}
```

NUnit may discover or schedule more scenarios, but `BrowserExecutionLimiter` acquires a slot before launching a browser. Therefore, at most three independent browsers are open at one time. Set `PARALLEL_ENABLED=false` to run serially or override the limit with `PARALLEL_WORKERS`.

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

## Test categories

Reqnroll tags are exposed as NUnit test categories:

- `smoke` — critical, non-mutating checks for valid login, Dashboard, navigation, and logout.
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

## Layer responsibilities

1. A `When` step calls a page action and stores its actual result in `ScenarioTestContext` when needed.
2. A `Then` step compares the actual result with values from `TestData` using NUnit or Playwright assertions.
3. Page classes do not contain NUnit or Playwright assertions.

See `Documentation/HardcodingAudit.md` for the project-wide hardcoding review and the intended location of each value type.
