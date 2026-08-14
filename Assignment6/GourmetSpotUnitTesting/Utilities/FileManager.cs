using GourmetSpot.Exceptions;

namespace GourmetSpot.Utilities
{
    public static class FileManager
    {
        public const string DataDirectory = "Data";
        public const string BillsDirectory = "Bills";
        public const string InventoryFilePath = DataDirectory + "/Inventory.txt";
        public const string MenuFilePath = DataDirectory + "/Menu.json";
        public const string OrdersFilePath = DataDirectory + "/Orders.txt";
        public const string ReservationsFilePath = DataDirectory + "/Reservations.txt";

        public static string LastErrorMessage { get; private set; } = string.Empty;

        public static bool EnsureApplicationDirectoriesExist(out string errorMessage)
        {
            try
            {
                if (!EnsureDirectoryExistsAndWritable(DataDirectory, out errorMessage))
                {
                    LastErrorMessage = errorMessage;
                    return false;
                }
                if (!EnsureDirectoryExistsAndWritable(BillsDirectory, out errorMessage))
                {
                    LastErrorMessage = errorMessage;
                    return false;
                }
                errorMessage = string.Empty;
                LastErrorMessage = string.Empty;
                return true;
            }
            catch (IOException ex)
            {
                errorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException("Unable to create application folders.", ex));
            }
            catch (Exception ex)
            {
                errorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException("Unexpected error while creating application folders.", ex));
            }
            LastErrorMessage = errorMessage;
            return false;
        }

        private static bool EnsureDirectoryExistsAndWritable(string directoryPath, out string errorMessage)
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
                TryAllowCurrentUserToWriteDirectory(directoryPath);
                string testFilePath = Path.Combine(directoryPath, $".write-test-{Guid.NewGuid():N}.tmp");
                File.WriteAllText(testFilePath, "");
                File.Delete(testFilePath);
                errorMessage = string.Empty;
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                errorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Application folder '{directoryPath}' is not writable.", ex));
            }
            catch (IOException ex)
            {
                errorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unable to verify write access for folder '{directoryPath}'.", ex));
            }
            catch (Exception ex)
            {
                errorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unexpected error while checking folder '{directoryPath}'.", ex));
            }
            return false;
        }

        private static void TryAllowCurrentUserToWriteDirectory(string directoryPath)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
                if (!OperatingSystem.IsWindows())
                {
                    UnixFileMode directoryMode = File.GetUnixFileMode(directoryPath);
                    File.SetUnixFileMode(
                        directoryPath,
                        directoryMode | UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }
            }
            catch (Exception ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unable to update folder permission for '{directoryPath}'.", ex));
            }
        }

        public static string GetBillFilePath(int orderId)
        {
            return $"{BillsDirectory}/Bill_{orderId}.txt";
        }

        public static bool TryReadAllLines(string filePath, out string[] lines)
        {
            lines = Array.Empty<string>();
            try
            {
                if (!File.Exists(filePath))
                {
                    LastErrorMessage = string.Empty;
                    return false;
                }
                lines = File.ReadAllLines(filePath);
                LastErrorMessage = string.Empty;
                return true;
            }
            catch (IOException ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unable to read file '{filePath}'.", ex));
            }
            catch (Exception ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unexpected error while reading file '{filePath}'.", ex));
            }
            return false;
        }

        public static bool TryWriteAllLines(string filePath, List<string> lines)
        {
            try
            {
                CreateParentDirectory(filePath);
                File.WriteAllLines(filePath, lines);
                LastErrorMessage = string.Empty;
                return true;
            }
            catch (IOException ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unable to write file '{filePath}'.", ex));
            }
            catch (Exception ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unexpected error while writing file '{filePath}'.", ex));
            }
            return false;
        }

        public static bool TryReadAllText(string filePath, out string content)
        {
            content = "";
            try
            {
                if (!File.Exists(filePath))
                {
                    LastErrorMessage = string.Empty;
                    return false;
                }
                content = File.ReadAllText(filePath);
                LastErrorMessage = string.Empty;
                return true;
            }
            catch (IOException ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unable to read file '{filePath}'.", ex));
            }
            catch (Exception ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unexpected error while reading file '{filePath}'.", ex));
            }
            return false;
        }

        public static bool TryWriteAllText(string filePath, string content)
        {
            try
            {
                CreateParentDirectory(filePath);
                File.WriteAllText(filePath, content);
                LastErrorMessage = string.Empty;
                return true;
            }
            catch (IOException ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unable to write file '{filePath}'.", ex));
            }
            catch (Exception ex)
            {
                LastErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException($"Unexpected error while writing file '{filePath}'.", ex));
            }
            return false;
        }

        private static void CreateParentDirectory(string filePath)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
