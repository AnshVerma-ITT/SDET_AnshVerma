using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightApiFramework.Framework.Assertions;

public static class ApiAssert
{
    public static void Status(IAPIResponse response, string endpoint, int expectedStatus, string message = "")
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            message = "Expected endpoint " + endpoint + " to return status " + expectedStatus + ".";
        }
        Assert.That(response.Status, Is.EqualTo(expectedStatus), message);
    }

    public static void HeaderContains(string actualValue, string headerName, string expectedValue)
    {
        Assert.That(actualValue, Does.Contain(expectedValue), "Expected header " + headerName + " to contain " + expectedValue + ".");
    }

    public static void FieldEquals<T>(T actualValue, T expectedValue, string fieldName)
    {
        Assert.That(actualValue, Is.EqualTo(expectedValue), "Expected response field " + fieldName + " to be " + expectedValue + ".");
    }

    public static void FieldContains(string actualValue, string expectedValue, string fieldName)
    {
        Assert.That(actualValue, Does.Contain(expectedValue), "Expected response field " + fieldName + " to contain " + expectedValue + ".");
    }

    public static void FieldNotEmpty(string actualValue, string fieldName)
    {
        Assert.That(actualValue, Is.Not.Empty, "Expected response field " + fieldName + " to have a value.");
    }

    public static void GreaterThanZero(int actualValue, string fieldName)
    {
        Assert.That(actualValue, Is.GreaterThan(0), "Expected response field " + fieldName + " to be greater than zero.");
    }

    public static void ArrayNotEmpty(int count, string fieldName)
    {
        Assert.That(count, Is.GreaterThan(0), "Expected response array " + fieldName + " to contain at least one item.");
    }

    public static void EmptyBody(string actualBody, string endpoint)
    {
        Assert.That(actualBody, Is.EqualTo(string.Empty), "Expected endpoint " + endpoint + " to return an empty response body.");
    }
}
