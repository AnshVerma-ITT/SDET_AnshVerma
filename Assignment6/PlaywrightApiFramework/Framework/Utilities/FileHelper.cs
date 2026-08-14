namespace PlaywrightApiFramework.Framework.Utilities;

public static class FileHelper
{
    public static string FindFile(string relativePath)
    {
        var path = FindFileOrEmpty(relativePath);
        if (path == "")
        {
            throw new FileNotFoundException("File not found: " + relativePath);
        }
        return path;
    }

    public static string FindFileOrEmpty(string relativePath)
    {
        var folder = Directory.GetCurrentDirectory();
        while (folder != null)
        {
            var path = Path.Combine(folder, relativePath);
            if (File.Exists(path))
            {
                return path;
            }
            folder = Directory.GetParent(folder)?.FullName;
        }
        return "";
    }
}
