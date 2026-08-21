using GourmetSpot.Exceptions;

namespace GourmetSpot.Utilities
{
    public static class ExceptionUtilities
    {
        public static string GetMessage(GourmetSpotException exception)
        {
            if (exception.InnerException == null)
            {
                return exception.Message;
            }
            return exception.Message + " " + exception.InnerException.Message;
        }

        public static void ShowError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.Error.WriteLine(message);
            }
        }
    }
}
