using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Playwright;

namespace PlaywrightApiFramework.Framework.Utilities;

public static class JsonHelper
{
    public static async Task<JsonElement> GetJson(IAPIResponse response)
    {
        var text = await response.TextAsync();
        var document = JsonDocument.Parse(text);
        return document.RootElement.Clone();
    }

    public static async Task<T> Deserialize<T>(IAPIResponse response)
    {
        var text = await response.TextAsync();
        return JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        })!;
    }

    public static string GetString(JsonElement json, string propertyName)
    {
        return json.GetProperty(propertyName).GetString() ?? "";
    }

    public static int GetInt(JsonElement json, string propertyName)
    {
        var property = json.GetProperty(propertyName);
        if (property.ValueKind == JsonValueKind.String)
        {
            return int.Parse(property.GetString() ?? "0");
        }
        return property.GetInt32();
    }

    public static string GetHeader(IAPIResponse response, string headerName)
    {
        foreach (var header in response.Headers)
        {
            if (string.Equals(header.Key, headerName, StringComparison.OrdinalIgnoreCase))
            {
                return header.Value;
            }
        }
        return "";
    }
}
