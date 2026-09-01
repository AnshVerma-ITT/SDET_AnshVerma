namespace SauceDemoBDD.TestData;

public sealed record Customer(string FirstName, string LastName, string PostalCode);
public sealed record CheckoutCase(Customer Customer, string ExpectedError);

public static class CheckoutTestData
{
    public static readonly Customer ValidCustomer = new("Rahul", "Verma", "560001");
    public const string ConfirmationMessage = "Thank you for your order!";

    public static CheckoutCase GetCase(string caseName)
    {
        return caseName.ToLowerInvariant() switch
        {
            "missing first name" => new(
                new Customer(string.Empty, ValidCustomer.LastName, ValidCustomer.PostalCode),
                "First Name is required"),
            "missing last name" => new(
                new Customer(ValidCustomer.FirstName, string.Empty, ValidCustomer.PostalCode),
                "Last Name is required"),
            "missing postal code" => new(
                new Customer(ValidCustomer.FirstName, ValidCustomer.LastName, string.Empty),
                "Postal Code is required"),
            _ => throw new ArgumentException($"Unknown checkout case: {caseName}")
        };
    }
}
