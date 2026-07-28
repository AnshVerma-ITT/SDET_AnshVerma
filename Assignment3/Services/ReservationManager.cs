using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class ReservationManager
    {
        private const int ReservationWindowHours = 2;
        private readonly List<Reservation> reservations;
        private readonly List<int> restaurantTableNumbers;
        private readonly string reservationsFilePath = ApplicationStorage.ReservationsFilePath;

        public ReservationManager()
        {
            reservations = new List<Reservation>();
            restaurantTableNumbers = new List<int>();

            for (int tableNumber = 1; tableNumber <= 10; tableNumber++)
            {
                restaurantTableNumbers.Add(tableNumber);
            }

            LoadReservations();
        }

        public int GetNextReservationId()
        {
            int nextReservationId = 1;

            foreach (Reservation reservation in reservations)
            {
                if (reservation.ReservationId >= nextReservationId)
                {
                    nextReservationId = reservation.ReservationId + 1;
                }
            }

            return nextReservationId;
        }

        public void AddReservation(Reservation reservation)
        {
            if (!IsTableAvailable(reservation.TableNumber, reservation.ReservationDateTime))
            {
                Console.WriteLine("This table is already reserved for the selected time window.");
                return;
            }

            reservations.Add(reservation);
            SaveReservations();
            Console.WriteLine("Reservation created successfully.");
        }

        public void DisplayAvailableTables(DateTime reservationDateTime)
        {
            List<int> availableTableNumbers = GetAvailableTables(reservationDateTime);

            if (availableTableNumbers.Count == 0)
            {
                Console.WriteLine("No tables are available for the selected time window.");
                return;
            }

            Console.WriteLine($"\nAvailable Tables for {reservationDateTime:dd-MM-yyyy HH:mm} to {reservationDateTime.AddHours(ReservationWindowHours):HH:mm}:");

            foreach (int tableNumber in availableTableNumbers)
            {
                Console.WriteLine($"Table {tableNumber}");
            }
        }

        public bool HasAvailableTables(DateTime reservationDateTime)
        {
            return GetAvailableTables(reservationDateTime).Count > 0;
        }

        public bool IsTableAvailable(int tableNumber, DateTime reservationDateTime)
        {
            if (!restaurantTableNumbers.Contains(tableNumber))
            {
                return false;
            }

            DateTime requestedStartTime = reservationDateTime;
            DateTime requestedEndTime = reservationDateTime.AddHours(ReservationWindowHours);

            foreach (Reservation reservation in reservations)
            {
                bool sameTable = reservation.TableNumber == tableNumber;
                bool reservationIsBooked = reservation.Status.Equals("Booked", StringComparison.OrdinalIgnoreCase);
                bool reservationTimeOverlaps = requestedStartTime < reservation.ReservationDateTime.AddHours(ReservationWindowHours) &&
                                               reservation.ReservationDateTime < requestedEndTime;

                if (sameTable && reservationIsBooked && reservationTimeOverlaps)
                {
                    return false;
                }
            }

            return true;
        }

        public void DisplayReservations()
        {
            if (reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }

            Console.WriteLine("\n========== RESERVATIONS ==========");

            foreach (Reservation reservation in reservations)
            {
                DisplayReservation(reservation);
            }
        }

        private void DisplayReservation(Reservation reservation)
        {
            Console.WriteLine(
                $"{reservation.ReservationId} - {reservation.CustomerName} - Contact: {reservation.ContactNumber} - Table {reservation.TableNumber} - {reservation.NumberOfGuests} guests - {reservation.ReservationDateTime:dd-MM-yyyy HH:mm} to {reservation.ReservationDateTime.AddHours(ReservationWindowHours):HH:mm} - {reservation.Status}");
        }

        public Reservation? SearchReservationById(int reservationId)
        {
            foreach (Reservation reservation in reservations)
            {
                if (reservation.ReservationId == reservationId)
                {
                    return reservation;
                }
            }

            return null;
        }

        public bool CancelReservation(int reservationId)
        {
            Reservation? reservation = SearchReservationById(reservationId);

            if (reservation == null)
            {
                return false;
            }

            reservation.Status = "Cancelled";
            SaveReservations();
            return true;
        }

        private List<int> GetAvailableTables(DateTime reservationDateTime)
        {
            List<int> availableTableNumbers = new List<int>();

            foreach (int tableNumber in restaurantTableNumbers)
            {
                if (IsTableAvailable(tableNumber, reservationDateTime))
                {
                    availableTableNumbers.Add(tableNumber);
                }
            }

            return availableTableNumbers;
        }

        private void SaveReservations()
        {
            List<string> reservationLines = new List<string>();

            foreach (Reservation reservation in reservations)
            {
                reservationLines.Add(
                    $"{reservation.ReservationId}|{reservation.CustomerName}|{reservation.ContactNumber}|{reservation.TableNumber}|{reservation.NumberOfGuests}|{reservation.ReservationDateTime:O}|{reservation.Status}");
            }

            ApplicationStorage.TryWriteAllLines(reservationsFilePath, reservationLines);
        }

        private void LoadReservations()
        {
            if (!ApplicationStorage.TryReadAllLines(reservationsFilePath, out string[] reservationLines))
            {
                return;
            }

            foreach (string reservationLine in reservationLines)
            {
                if (string.IsNullOrWhiteSpace(reservationLine))
                {
                    continue;
                }

                string[] reservationData = reservationLine.Split('|');

                if (reservationData.Length < 7)
                {
                    continue;
                }

                bool reservationIdValid = int.TryParse(reservationData[0], out int reservationId);
                bool tableNumberValid = int.TryParse(reservationData[3], out int tableNumber);
                bool guestCountValid = int.TryParse(reservationData[4], out int numberOfGuests);
                bool dateTimeValid = DateTime.TryParse(reservationData[5], out DateTime reservationDateTime);

                if (!reservationIdValid || !tableNumberValid || !guestCountValid || !dateTimeValid)
                {
                    continue;
                }

                string reservationStatus = reservationData[6];

                try
                {
                    Reservation reservation = new Reservation(
                        reservationId,
                        reservationData[1],
                        reservationData[2],
                        tableNumber,
                        numberOfGuests,
                        reservationDateTime,
                        reservationStatus,
                        false);

                    reservations.Add(reservation);
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
        }
    }
}
