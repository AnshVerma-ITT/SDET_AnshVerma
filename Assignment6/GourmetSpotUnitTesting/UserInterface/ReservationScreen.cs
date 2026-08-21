using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.UserInterface.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class ReservationScreen : IDisplay
    {
        public ReservationManager reservationManager;

        public ReservationScreen(ReservationManager reservationManager)
        {
            this.reservationManager = reservationManager;
        }

        public void Display()
        {
            while (true)
            {
                DisplayMenu();
                string userChoice = ConsoleInput.ReadMenuChoice();
                switch (userChoice)
                {
                    case "1":
                        CreateReservation();
                        break;
                    case "2":
                        DisplayReservations();
                        break;
                    case "3":
                        SearchReservationById();
                        break;
                    case "4":
                        CancelReservation();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== Reservation Management =====");
            Console.WriteLine("1. Create Reservation");
            Console.WriteLine("2. View Reservations");
            Console.WriteLine("3. Search Reservation");
            Console.WriteLine("4. Cancel Reservation");
            Console.WriteLine("5. Back");
            Console.Write("Enter your choice: ");
        }

        private void CreateReservation()
        {
            int reservationId = reservationManager.GetNextReservationId();
            Console.WriteLine($"Reservation ID: {reservationId}");
            string customerName = ConsoleInput.ReadCustomerName();
            string contactNumber = ConsoleInput.ReadContactNumber();
            int numberOfGuests = ConsoleInput.ReadPositiveInt("Enter Number of Guests: ");
            DateTime reservationDateTime = ConsoleInput.ReadReservationDateTime();
            if (!reservationManager.HasAvailableTables(reservationDateTime))
            {
                Console.WriteLine("No tables are available for the selected time window.");
                return;
            }
            DisplayAvailableTables(reservationDateTime);
            int tableNumber = ReadAvailableTableNumber(reservationDateTime);
            Reservation reservation = new Reservation(
                reservationId,
                customerName,
                contactNumber,
                tableNumber,
                numberOfGuests,
                reservationDateTime);
            reservationManager.AddReservation(reservation, out string reservationMessage);
            Console.WriteLine(reservationMessage);
        }

        private int ReadAvailableTableNumber(DateTime reservationDateTime)
        {
            while (true)
            {
                int tableNumber = ConsoleInput.ReadPositiveInt("Select Table Number: ");
                if (reservationManager.IsTableAvailable(tableNumber, reservationDateTime))
                {
                    return tableNumber;
                }
                Console.WriteLine("This table is not available for the selected time window. Please select from available tables.");
            }
        }

        private void DisplayAvailableTables(DateTime reservationDateTime)
        {
            List<int> availableTableNumbers = reservationManager.GetAvailableTables(reservationDateTime);
            if (availableTableNumbers.Count == 0)
            {
                Console.WriteLine("No tables are available for the selected time window.");
                return;
            }
            Console.WriteLine($"\nAvailable Tables for {reservationDateTime:dd-MM-yyyy HH:mm} to {reservationDateTime.AddHours(reservationManager.ReservationWindowDurationHours):HH:mm}:");
            foreach (int tableNumber in availableTableNumbers)
            {
                Console.WriteLine($"Table {tableNumber}");
            }
        }

        private void DisplayReservations()
        {
            List<Reservation> reservations = reservationManager.GetAllReservations();
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

        private void SearchReservationById()
        {
            int reservationId = ConsoleInput.ReadPositiveInt("Enter Reservation ID: ");
            Reservation? reservation = reservationManager.SearchReservationById(reservationId);
            if (reservation != null)
            {
                Console.WriteLine("\nReservation Found");
                Console.WriteLine("-------------------------");
                DisplayReservation(reservation);
            }
            else
            {
                Console.WriteLine("Reservation not found.");
            }
        }

        private void CancelReservation()
        {
            int reservationId = ConsoleInput.ReadPositiveInt("Enter Reservation ID: ");
            reservationManager.CancelReservation(reservationId, out string cancelMessage);
            Console.WriteLine(cancelMessage);
        }

        private void DisplayReservation(Reservation reservation)
        {
            Console.WriteLine(
                $"{reservation.ReservationId} - {reservation.CustomerName} - Contact: {reservation.ContactNumber} - Table {reservation.TableNumber} - {reservation.NumberOfGuests} guests - {reservation.ReservationDateTime:dd-MM-yyyy HH:mm} to {reservation.ReservationDateTime.AddHours(reservationManager.ReservationWindowDurationHours):HH:mm} - {reservation.Status}");
        }
    }
}
