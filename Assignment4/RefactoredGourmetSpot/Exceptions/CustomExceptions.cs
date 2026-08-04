namespace GourmetSpot.Exceptions
{
    public abstract class GourmetSpotException : Exception
    {
        public GourmetSpotException(string message)
            : base(message)
        {
        }

        public GourmetSpotException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class FileOperationException : GourmetSpotException
    {
        public FileOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class MenuException : GourmetSpotException
    {
        public MenuException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ReservationException : GourmetSpotException
    {
        public ReservationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class OrderException : GourmetSpotException
    {
        public OrderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class InputException : GourmetSpotException
    {
        public InputException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
