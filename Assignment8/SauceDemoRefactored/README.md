# SauceDemo Refactored with POM

This folder is a self-contained Playwright and NUnit project for Assignment 1.

## Structure

```text
SauceDemoRefactored
├── Configuration       # Browser matrix, routes, and JSON settings
├── Enums               # Product sorting choices
├── Extensions          # Reusable mappings
├── Fixtures            # Browser setup, evidence, and shared flows
├── Infrastructure      # Reusable navigation command
├── Pages               # Page Object Model classes
├── TestData            # Test inputs and expected values
├── Tests               # NUnit tests
├── allureConfig.json
├── appsettings.json
└── SauceDemoRefactored.csproj
```

## Run

From the workspace root:

```bash
dotnet restore SauceDemoRefactored/SauceDemoRefactored.csproj
dotnet build SauceDemoRefactored/SauceDemoRefactored.csproj
pwsh SauceDemoRefactored/bin/Debug/net10.0/playwright.ps1 install chromium webkit
BROWSER=Chromium dotnet test SauceDemoRefactored/SauceDemoRefactored.csproj --filter "TestCategory!=FailureDemo"
```

The project contains POM-based login, product, cart, checkout, and product-detail tests. Advanced Playwright feature demonstrations are maintained separately in `PlaywrightAdvancedFeatures`.

Traces, videos, and screenshots are written to `SauceDemoRefactored/TestResults`. Allure raw results are written to `SauceDemoRefactored/Reports/allure-results`.
