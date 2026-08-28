namespace SauceDemoBDD.Support;

public sealed class ScenarioState
{
    public List<string> SelectedProducts { get; } = [];
    public string ExpectedLoginError { get; set; } = string.Empty;
    public string ExpectedCheckoutError { get; set; } = string.Empty;
}
