namespace SauceDemo.Playwright.Tests.Configuration;

public static class AppRoutes
{
    public const string Root = "/";
    public const string Inventory = "/inventory.html";
    public const string ProductDetails = "/inventory-item.html";
    public const string Cart = "/cart.html";
    public const string CheckoutStepOne = "/checkout-step-one.html";
    public const string CheckoutStepTwo = "/checkout-step-two.html";
    public const string CheckoutComplete = "/checkout-complete.html";

    public static string GetProductDetailsRoute(int productId)
    {
        return $"{ProductDetails}?id={productId}";
    }
}
