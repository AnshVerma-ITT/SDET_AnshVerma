namespace PlaywrightApiFramework.Framework.Utilities;

public static class DataGenerator
{
    public static string RandomText(string prefix)
    {
        var random = new Random();
        var number = random.Next(1000, 9999);
        return prefix + "_" + number;
    }
}
