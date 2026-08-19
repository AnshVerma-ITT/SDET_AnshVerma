using PlaywrightApiFramework.Framework.Data;
using PlaywrightApiFramework.ReqRes.Models;

namespace PlaywrightApiFramework.Tests.DataProviders;

public static class UserDataProvider
{
    public static UserScenarioData ScenarioData()
    {
        return TestDataHelper.ReadJson<UserScenarioData>("ReqRes/DataFiles/user-scenarios.json");
    }

    public static List<User> JsonUsers()
    {
        return TestDataHelper.ReadJsonList<User>("ReqRes/DataFiles/users.json");
    }

    public static List<User> CsvUsers()
    {
        var rows = TestDataHelper.ReadCsv("ReqRes/DataFiles/users.csv");
        var users = new List<User>();
        foreach (var row in rows)
        {
            users.Add(new User
            {
                Name = row["name"],
                Job = row["job"]
            });
        }
        return users;
    }
}
