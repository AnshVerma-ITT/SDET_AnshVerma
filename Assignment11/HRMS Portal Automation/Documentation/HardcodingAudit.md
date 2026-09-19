# Project-wide hardcoding audit

All project-owned source, feature, configuration, and documentation files were reviewed. The goal is not to eliminate every literal: selectors, protocol names, and specification text must exist somewhere. The goal is to keep each value in the layer that owns it and avoid duplicated business data.

| Area reviewed | Hardcoding found | Resolution |
| --- | --- | --- |
| `Pages/*.cs` | Selectors, accessible names, attributes, date formats, keyboard keys, generated descriptions, expected employee data, expected messages, colors, pagination sizes, and social URLs | Selectors and UI mechanics are private constants at the top of each page class. Expected values and scenario inputs moved to `TestData`. Pages now return actual values and contain no assertions. |
| `StepDefinitions/*.cs` | Expected names, messages, colors, status text, page sizes, dates, and no-op `Then` steps | Business expectations now come from `TestData`. Assertions are implemented in `Then` steps; the previous no-op steps were removed. Assertion-only attribute names remain local constants because they describe the DOM state being asserted. |
| `TestData/*.cs` | Scattered expected business and account-specific data | Consolidated into focused files for login, dashboard, profile, attendance, employee directory, footer, leave application, leave correction, and resignation. These are deliberately centralized test expectations. |
| `Configuration/*.cs` | URLs/routes, browser names, environment-variable names, and mutating-scenario tags | Base URL and execution settings remain external in `appsettings.json` with environment overrides. Names/routes/tags are named constants. Credentials are read only from environment variables. |
| `Helper/*.cs` | Browser channel names and an unused worker-count setting | Channel names are constants. `parallelWorkers` now actively controls a process-wide browser execution limiter. |
| `Hooks/TestHooks.cs` | Literal leave tags and per-scenario browser creation | Tags moved to `Configuration/TestTags.cs`. Per-scenario isolation remains intentional, with a maximum of three active browsers. |
| `Support/TestContext.cs` | A vague folder name for scenario-scoped dependency/state sharing | Renamed to `Context/ScenarioTestContext.cs`; result state is strongly typed through `Models/ScenarioResults.cs`. |
| `Features/*.feature` | Scenario wording and business-readable examples | Retained intentionally as executable specification text. Runtime values are supplied by `TestData` or configuration rather than duplicated in page classes. |
| `HRIntimeAutomation.csproj` | Package versions and target framework | Retained intentionally. Pinned dependency versions are build metadata required for reproducible restores. |
| `appsettings.json`, `.env.example`, `reqnroll.json`, `allureConfig.json` | Default runtime settings and output paths | Retained as external configuration. The visible-browser concurrency limit is `parallelWorkers: 3`; sensitive values are not stored. |
| `README.md`, `Documentation/*.md`, `.editorconfig`, `.gitignore`, artifact `.gitkeep` files | Commands, guidance, formatting rules, and ignored output patterns | Retained intentionally because these literals document or control tooling rather than test behavior. |

## Placement rules going forward

- Put environment-dependent values in `appsettings.json` and expose environment-variable overrides in `TestSettings`.
- Put credentials only in environment variables.
- Put expected business/account data in the relevant `TestData` class.
- Put selectors and stable UI labels in private constants at the top of the owning page class.
- Put UI actions and queries in `Pages`; put NUnit/Playwright assertions in `StepDefinitions`.
- Do not hardcode generated Mantine IDs. Resolve them through semantic locators or `aria-controls` at runtime.
