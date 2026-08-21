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
        public FileOperationException(string message)
            : base(message)
        {
        }

        public FileOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class MenuException : GourmetSpotException
    {
        public MenuException(string message)
            : base(message)
        {
        }

        public MenuException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ReservationException : GourmetSpotException
    {
        public ReservationException(string message)
            : base(message)
        {
        }

        public ReservationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class OrderException : GourmetSpotException
    {
        public OrderException(string message)
            : base(message)
        {
        }

        public OrderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class InputException : GourmetSpotException
    {
        public InputException(string message)
            : base(message)
        {
        }

        public InputException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
        public class BillException : GourmetSpotException
    {
        public BillException(string message)
            : base(message)
        {
        }

        public BillException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
