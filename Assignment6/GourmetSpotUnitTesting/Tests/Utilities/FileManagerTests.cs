using GourmetSpot.Tests.Helpers;
using GourmetSpot.Utilities;

namespace GourmetSpot.Tests.Utilities
{
    public class FileManagerTests : FileTestBase
    {
        [Test]
        public void TryWriteAllTextAndTryReadAllText_WhenFileExists_ReadsContent()
        {
            string expectedContent = "menu";
            bool written = FileManager.TryWriteAllText(
                FileManager.MenuFilePath,
                expectedContent);
            bool read = FileManager.TryReadAllText(
                FileManager.MenuFilePath,
                out string content);
            Assert.That(written, Is.True);
            Assert.That(read, Is.True);
            Assert.That(content, Is.EqualTo(expectedContent));
        }
    }
}
