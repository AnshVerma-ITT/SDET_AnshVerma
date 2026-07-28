namespace GourmetSpot.Services
{
    public static class ApplicationStorage
    {
        public const string DataDirectory = "Data";
        public const string BillsDirectory = "Bills";
        public const string InventoryFilePath = DataDirectory + "/inventory.txt";
        public const string MenuFilePath = DataDirectory + "/menu.json";
        public const string OrdersFilePath = DataDirectory + "/orders.txt";
        public const string ReservationsFilePath = DataDirectory + "/reservations.txt";

        public static void EnsureApplicationDirectoriesExist()
        {
            try
            {
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(BillsDirectory);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Unable to create application folders: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied while creating application folders: {ex.Message}");
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
                    return false;
                }

                lines = File.ReadAllLines(filePath);
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Unable to read file '{filePath}': {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied while reading file '{filePath}': {ex.Message}");
            }

            return false;
        }

        public static bool TryWriteAllLines(string filePath, List<string> lines)
        {
            try
            {
                File.WriteAllLines(filePath, lines);
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Unable to write file '{filePath}': {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied while writing file '{filePath}': {ex.Message}");
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
                    return false;
                }

                content = File.ReadAllText(filePath);
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Unable to read file '{filePath}': {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied while reading file '{filePath}': {ex.Message}");
            }

            return false;
        }

        public static bool TryWriteAllText(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Unable to write file '{filePath}': {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission denied while writing file '{filePath}': {ex.Message}");
            }

            return false;
        }
    }
}
