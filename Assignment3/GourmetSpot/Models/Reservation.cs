namespace GourmetSpot.Models
{
    public class Reservation
    {
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
            string status = "Booked")
        {
            ReservationId = reservationId;
            CustomerName = customerName;
            ContactNumber = contactNumber;
            TableNumber = tableNumber;
            NumberOfGuests = numberOfGuests;
            ReservationDateTime = reservationDateTime;
            Status = status;
        }
    }
}
