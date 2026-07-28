using System.Globalization;

namespace GourmetSpot.UserInterface
{
    public static class ConsoleInput
    {
        private const int ContactNumberLength = 10;
        private const int MaximumAdvanceReservationMonths = 3;
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
            while (true)
            {
                try
                {
                    string customerName = ReadRequiredText("Enter Customer Name: ");

                    foreach (char letter in customerName)
                    {
                        if (!char.IsLetter(letter) && letter != ' ')
                        {
                            throw new ArgumentException("Customer name should contain alphabets only.");
                        }
                    }

                    return customerName;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static string ReadContactNumber()
        {
            while (true)
            {
                try
                {
                    string contactNumber = ReadRequiredText("Enter Contact Number: ");

                    if (contactNumber.Length != ContactNumberLength)
                    {
                        throw new ArgumentException("Contact number must contain exactly 10 digits.");
                    }

                    foreach (char digit in contactNumber)
                    {
                        if (!char.IsDigit(digit))
                        {
                            throw new ArgumentException("Contact number must contain digits only.");
                        }
                    }

                    return contactNumber;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
                try
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
                        throw new ArgumentException($"Please enter date and time in {ReservationDateTimeFormat} format.");
                    }

                    DateTime now = DateTime.Now;
                    DateTime maximumReservationDate = now.AddMonths(MaximumAdvanceReservationMonths);

                    if (reservationDateTime < now)
                    {
                        throw new ArgumentException("Reservation date and time cannot be in the past.");
                    }

                    if (reservationDateTime > maximumReservationDate)
                    {
                        throw new ArgumentException("Reservation date and time cannot be more than 3 months from now.");
                    }

                    return reservationDateTime;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
                Console.WriteLine($"Unable to read input: {ex.Message}");
                return "";
            }
        }
    }
}
