# SauceDemo BDD

This is a self-contained Playwright, Page Object Model, and BDD assignment. It does not reference the Assignment 1 project.

## Structure

```text
SauceDemoBDD
├── Configuration       # URL and JSON settings
├── Features            # Authentication, inventory/cart, and checkout Gherkin
├── Flows               # Reusable login journey
├── Pages               # Page Object Model classes
├── StepDefinitions     # Given, When, and Then bindings
├── Support             # Browser driver and hooks
├── TestData            # Central login, product, and checkout data
├── allureConfig.json
├── appsettings.json
└── SauceDemoBDD.csproj
```

Reqnroll provides the maintained SpecFlow-compatible BDD integration for modern .NET. It generates NUnit tests from the feature files during the build.

## Coverage

- Successful standard-user login
- Invalid username and password
- Missing username and password
- Locked user
- Price sorting
- Add, remove, and retain cart products
- Complete Week 6 purchase journey
- Missing first name, last name, and postal code

Selectors exist only inside page objects. Routes, browser settings, credentials, product aliases, customer values, expected errors, and evidence directory names are centralized. Shared login behavior is implemented once in `LoginFlow`, and per-scenario data is held in `ScenarioState`.

## Run

```bash
dotnet restore SauceDemoBDD/SauceDemoBDD.csproj
dotnet build SauceDemoBDD/SauceDemoBDD.csproj
pwsh SauceDemoBDD/bin/Debug/net10.0/playwright.ps1 install chromium webkit
dotnet test SauceDemoBDD/SauceDemoBDD.csproj --filter "TestCategory=bdd"
```

Chromium is used by default. Select WebKit with:

```bash
BROWSER=WebKit dotnet test SauceDemoBDD/SauceDemoBDD.csproj --filter "TestCategory=bdd"
```

Traces, videos, failure screenshots, and Allure raw results are written below `SauceDemoBDD/bin/Debug/net10.0` using the configured directory names.
