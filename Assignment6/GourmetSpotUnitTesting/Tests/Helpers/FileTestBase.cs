namespace GourmetSpot.Tests.Helpers
{
    public abstract class FileTestBase
    {
        private string originalDirectory = string.Empty;
        private string testDirectory = string.Empty;

        [SetUp]
        public void SetUpFileTest()
        {
            originalDirectory = Directory.GetCurrentDirectory();
            testDirectory = Path.Combine(
                Path.GetTempPath(),
                "GourmetSpotTests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(testDirectory);
            Directory.SetCurrentDirectory(testDirectory);
        }

        [TearDown]
        public void TearDownFileTest()
        {
            Directory.SetCurrentDirectory(originalDirectory);
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }
    }
}
