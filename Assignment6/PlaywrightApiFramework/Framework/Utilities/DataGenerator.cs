using PlaywrightApiFramework.ReqRes.Models;

namespace PlaywrightApiFramework.Framework.Utilities;

public static class DataGenerator
{
    public static User CreateUser()
    {
        var random = new Random();
        var number = random.Next(1000, 9999);
        return new User
        {
            Name = "student_" + number,
            Job = "QA Tester"
        };
    }
}
