using PlaywrightApiFramework.Framework.Data;
using PlaywrightApiFramework.ReqRes.Models;

namespace PlaywrightApiFramework.Tests.DataProviders;

public static class UserDataProvider
{
    public static UserScenarioData ScenarioData()
    {
        return TestDataHelper.ReadJson<UserScenarioData>("ReqRes/DataFiles/user-scenarios.json");
    }

    public static List<UserRequest> JsonUsers()
    {
        return TestDataHelper.ReadJsonList<UserRequest>("ReqRes/DataFiles/users.json");
    }

    public static List<UserRequest> CsvUsers()
    {
        var rows = TestDataHelper.ReadCsv("ReqRes/DataFiles/users.csv");
        var users = new List<UserRequest>();
        foreach (var row in rows)
        {
            users.Add(new UserRequest
            {
                Name = row["name"],
                Job = row["job"]
            });
        }
        return users;
    }
}
