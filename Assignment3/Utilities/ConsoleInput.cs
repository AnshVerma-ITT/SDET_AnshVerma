using System.Globalization;

namespace GourmetSpot.Utilities
{
    public static class ConsoleInput
    {
        private const string ReservationDateTimeFormat = "dd-MM-yyyy HH:mm";

        public static string ReadRequiredText(string message)
        {
            while (true)
            {
                Console.Write(message);
                string value = ReadConsoleLine();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
                Console.WriteLine("This field cannot be empty.");
            }
        }

        public static string ReadMenuChoice()
        {
            return ReadConsoleLine();
        }

        public static bool ReadYesNo(string message)
        {
            while (true)
            {
                Console.Write(message);
                string choice = ReadConsoleLine().Trim();
                if (choice.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    choice.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (choice.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                    choice.Equals("no", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                Console.WriteLine("Please enter y or n.");
            }
        }

        public static string ReadCustomerName()
        {
            return ReadRequiredText("Enter Customer Name: ");
        }

        public static string ReadContactNumber()
        {
            return ReadRequiredText("Enter Contact Number: ");
        }

        public static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = ReadConsoleLine();
                if (int.TryParse(input, out int value))
                {
                    return value;
                }
                Console.WriteLine("Please enter a valid number.");
            }
        }

        public static int ReadPositiveInt(string message)
        {
            while (true)
            {
                int value = ReadInt(message);
                if (value > 0)
                {
                    return value;
                }
                Console.WriteLine("Value must be greater than zero.");
            }
        }

        public static int ReadNonNegativeInt(string message)
        {
            while (true)
            {
                int value = ReadInt(message);
                if (value >= 0)
                {
                    return value;
                }
                Console.WriteLine("Value cannot be negative.");
            }
        }

        public static double ReadPositiveDouble(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = ReadConsoleLine();
                if (double.TryParse(input, out double value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter a value greater than zero.");
            }
        }

        public static double ReadNonNegativeDouble(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = ReadConsoleLine();
                if (double.TryParse(input, out double value) && value >= 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter zero or a positive value.");
            }
        }

        public static decimal ReadPositiveDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = ReadConsoleLine();
                if (decimal.TryParse(input, out decimal value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter a valid amount greater than zero.");
            }
        }

        public static DateTime ReadReservationDateTime()
        {
            while (true)
            {
                Console.Write($"Enter Reservation Date and Time ({ReservationDateTimeFormat}): ");
                string input = ReadConsoleLine();
                bool validDateTime = DateTime.TryParseExact(
                    input,
                    ReservationDateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime reservationDateTime);
                if (!validDateTime)
                {
                    Console.WriteLine($"Please enter date and time in {ReservationDateTimeFormat} format.");
                    continue;
                }
                return reservationDateTime;
            }
        }

        private static string ReadConsoleLine()
        {
            try
            {
                return Console.ReadLine() ?? "";
            }
            catch (IOException ex)
            {
                throw new IOException($"Unable to read console input: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while reading console input: {ex.Message}", ex);
            }
        }
    }
}
