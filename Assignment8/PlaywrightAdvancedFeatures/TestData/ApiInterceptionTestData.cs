using System.Net;
using PlaywrightAdvancedFeatures.Locators;

namespace PlaywrightAdvancedFeatures.TestData;

public static class ApiInterceptionTestData
{
    public const string RoutePattern = "**/api/products";
    public const string RequestUrl = "https://training.test/api/products";
    public const string JsonContentType = "application/json";
    public const string CorsHeaderName = "Access-Control-Allow-Origin";
    public const string CorsHeaderValue = "*";
    public const string LoadProductButtonText = "Load product";
    public const string MockProductName = "Mock Sauce Labs Backpack";
    public static readonly int SuccessStatusCode = (int)HttpStatusCode.OK;
    public static readonly string ResponseBody = $$"""{"name":"{{MockProductName}}"}""";

    public static readonly string PageContent = $$"""
        <button id="{{AdvancedFeatureLocators.LoadProductButtonId}}">{{LoadProductButtonText}}</button>
        <p id="{{AdvancedFeatureLocators.ProductNameId}}"></p>
        <script>
          document.querySelector('#{{AdvancedFeatureLocators.LoadProductButtonId}}').addEventListener('click', async () => {
            const response = await fetch('{{RequestUrl}}');
            const product = await response.json();
            document.querySelector('#{{AdvancedFeatureLocators.ProductNameId}}').textContent = product.name;
          });
        </script>
        """;
}
