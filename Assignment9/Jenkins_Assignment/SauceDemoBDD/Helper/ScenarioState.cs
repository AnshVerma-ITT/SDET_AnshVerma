namespace SauceDemoBDD.Support;

public sealed class ScenarioState
{
    private string? _expectedLoginError;
    private string? _expectedCheckoutError;

    public List<string> SelectedProducts { get; } = [];

    public string ExpectedLoginError
    {
        get => _expectedLoginError
            ?? throw new InvalidOperationException("Expected login error has not been set.");
        set => _expectedLoginError = value;
    }

    public string ExpectedCheckoutError
    {
        get => _expectedCheckoutError
            ?? throw new InvalidOperationException("Expected checkout error has not been set.");
        set => _expectedCheckoutError = value;
    }
}
