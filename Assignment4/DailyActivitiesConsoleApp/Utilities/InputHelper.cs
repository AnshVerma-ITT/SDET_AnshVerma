using System;

namespace Assignment1_OOP_DailyActivities.Utilities
{
    public class InputHelper
    {
        public static string ReadText(string message)
        {
            try
            {
                Console.Write(message);
                string value = Console.ReadLine();
                Validator.ValidateText(value, "Input");
                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int ReadNumber(string message)
        {
            try
            {
                Console.Write(message);
                string value = Console.ReadLine();
                Validator.ValidateText(value, "Number");
                return Convert.ToInt32(value);
            }
            catch (FormatException)
            {
                throw new Exception("Please enter numbers only.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
