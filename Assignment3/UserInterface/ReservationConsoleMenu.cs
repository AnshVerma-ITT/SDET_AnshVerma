using GourmetSpot.Models;
using GourmetSpot.Services;

namespace GourmetSpot.UserInterface
{
    public class ReservationConsoleMenu
    {
        private const int ReservationWindowHours = 2;
        private readonly ReservationManager reservationManager;

        public ReservationConsoleMenu(ReservationManager reservationManager)
        {
            this.reservationManager = reservationManager;
        }

        public void Show()
        {
            while (true)
            {
                DisplayReservationMenu();

                string userChoice = ConsoleInput.ReadMenuChoice();

                switch (userChoice)
                {
                    case "1":
                        CreateReservation();
                        break;
                    case "2":
                        reservationManager.DisplayReservations();
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

        private void DisplayReservationMenu()
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
            try
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

                reservationManager.DisplayAvailableTables(reservationDateTime);
                int tableNumber = ReadAvailableTableNumber(reservationDateTime);

                Reservation reservation = new Reservation(
                    reservationId,
                    customerName,
                    contactNumber,
                    tableNumber,
                    numberOfGuests,
                    reservationDateTime);

                reservationManager.AddReservation(reservation);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
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
            bool reservationCancelled = reservationManager.CancelReservation(reservationId);

            if (reservationCancelled)
            {
                Console.WriteLine("Reservation cancelled successfully.");
            }
            else
            {
                Console.WriteLine("Reservation not found.");
            }
        }

        private void DisplayReservation(Reservation reservation)
        {
            Console.WriteLine(
                $"{reservation.ReservationId} - {reservation.CustomerName} - Contact: {reservation.ContactNumber} - Table {reservation.TableNumber} - {reservation.NumberOfGuests} guests - {reservation.ReservationDateTime:dd-MM-yyyy HH:mm} to {reservation.ReservationDateTime.AddHours(ReservationWindowHours):HH:mm} - {reservation.Status}");
        }
    }
}
