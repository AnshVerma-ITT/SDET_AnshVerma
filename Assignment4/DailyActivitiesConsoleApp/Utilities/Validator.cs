using System;

namespace DailyActivityTracker.Utilities
{
    public class Validator
    {
        public static void ValidateText(string value, string fieldName)
        {
            try
            {
                if (value == null || value.Trim() == "")
                {
                    throw new Exception(fieldName + " cannot be empty.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void ValidateId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("Id must be greater than zero.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
