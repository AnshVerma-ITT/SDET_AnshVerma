namespace GourmetSpot.Models
{
    public class Reservation
    {
        private const int ReservationWindowHours = 2;
        private const int MaximumAdvanceReservationMonths = 3;

        public int ReservationId { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public int TableNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public string Status { get; set; }

        public Reservation(
            int reservationId,
            string customerName,
            string contactNumber,
            int tableNumber,
            int numberOfGuests,
            DateTime reservationDateTime,
            string status = "Booked",
            bool validateReservationDateTime = true)
        {
            if (!IsValidCustomerName(customerName))
            {
                throw new ArgumentException("Customer name should contain alphabets only.");
            }

            if (!IsValidContactNumber(contactNumber))
            {
                throw new ArgumentException("Contact number must contain exactly 10 digits.");
            }

            if (tableNumber <= 0)
            {
                throw new ArgumentException("Table number must be greater than zero.");
            }

            if (numberOfGuests <= 0)
            {
                throw new ArgumentException("Number of guests must be greater than zero.");
            }

            if (validateReservationDateTime && !IsValidReservationDateTime(reservationDateTime))
            {
                throw new ArgumentException("Reservation date and time cannot be in the past or more than 3 months from now.");
            }

            ReservationId = reservationId;
            CustomerName = customerName.Trim();
            ContactNumber = contactNumber.Trim();
            TableNumber = tableNumber;
            NumberOfGuests = numberOfGuests;
            ReservationDateTime = reservationDateTime;
            Status = status;
        }
        
        private bool IsValidCustomerName(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return false;
            }

            foreach (char letter in customerName.Trim())
            {
                if (!char.IsLetter(letter) && letter != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidContactNumber(string contactNumber)
        {
            string trimmedContactNumber = contactNumber.Trim();

            if (trimmedContactNumber.Length != 10)
            {
                return false;
            }

            foreach (char digit in trimmedContactNumber)
            {
                if (!char.IsDigit(digit))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidReservationDateTime(DateTime reservationDateTime)
        {
            DateTime now = DateTime.Now;
            DateTime maximumReservationDate = now.AddMonths(MaximumAdvanceReservationMonths);

            return reservationDateTime >= now && reservationDateTime <= maximumReservationDate;
        }
    }
}
