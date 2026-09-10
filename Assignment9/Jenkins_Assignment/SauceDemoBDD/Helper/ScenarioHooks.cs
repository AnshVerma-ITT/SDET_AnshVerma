using Reqnroll;

namespace SauceDemoBDD.Support;

[Binding]
public sealed class ScenarioHooks
{
    private readonly BrowserDriver _driver;
    private readonly ScenarioContext _scenarioContext;

    public ScenarioHooks(BrowserDriver driver, ScenarioContext scenarioContext)
    {
        _driver = driver;
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public Task StartBrowser()
    {
        return _driver.Start();
    }

    [AfterScenario]
    public Task StopBrowser()
    {
        return _driver.Stop(_scenarioContext.TestError is not null);
    }
}
