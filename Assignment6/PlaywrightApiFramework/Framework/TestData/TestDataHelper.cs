using System.Text.Json;
using PlaywrightApiFramework.Framework.Utilities;

namespace PlaywrightApiFramework.Framework.TestData;

public static class TestDataHelper
{
    public static List<T> ReadJsonList<T>(string relativePath)
    {
        var path = FileHelper.FindFile(relativePath);
        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<T>();
    }

    public static List<Dictionary<string, string>> ReadCsv(string relativePath)
    {
        var path = FileHelper.FindFile(relativePath);
        var lines = File.ReadAllLines(path);
        var data = new List<Dictionary<string, string>>();
        if (lines.Length == 0)
        {
            return data;
        }
        var headers = lines[0].Split(',');
        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var row = new Dictionary<string, string>();
            for (var j = 0; j < headers.Length; j++)
            {
                row[headers[j]] = values[j];
            }
            data.Add(row);
        }
        return data;
    }
}
