using NUnit.Framework;

namespace PlaywrightAdvancedFeatures.Infrastructure;

public static class TestPaths
{
    public static string InTestOutput(params string[] parts)
    {
        return Path.Combine([TestContext.CurrentContext.TestDirectory, .. parts]);
    }
}
