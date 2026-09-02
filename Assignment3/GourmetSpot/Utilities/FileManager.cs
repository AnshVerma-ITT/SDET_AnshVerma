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
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(BillsDirectory);
                errorMessage = string.Empty;
                LastErrorMessage = string.Empty;
                return true;
            }
            catch (IOException ex)
            {
                errorMessage = $"Unable to create application folders: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Unexpected error while creating application folders: {ex.Message}";
            }
            LastErrorMessage = errorMessage;
            return false;
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
                LastErrorMessage = $"Unable to read file '{filePath}': {ex.Message}";
            }
            catch (Exception ex)
            {
                LastErrorMessage = $"Unexpected error while reading file '{filePath}': {ex.Message}";
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
                LastErrorMessage = $"Unable to write file '{filePath}': {ex.Message}";
            }
            catch (Exception ex)
            {
                LastErrorMessage = $"Unexpected error while writing file '{filePath}': {ex.Message}";
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
                LastErrorMessage = $"Unable to read file '{filePath}': {ex.Message}";
            }
            catch (Exception ex)
            {
                LastErrorMessage = $"Unexpected error while reading file '{filePath}': {ex.Message}";
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
                LastErrorMessage = $"Unable to write file '{filePath}': {ex.Message}";
            }
            catch (Exception ex)
            {
                LastErrorMessage = $"Unexpected error while writing file '{filePath}': {ex.Message}";
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
