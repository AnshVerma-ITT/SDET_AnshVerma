using System.Globalization;
using GourmetSpot.Exceptions;
using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class ReservationManager : IStoreManager<Reservation>
    {
        public const int ReservationWindowHours = 2;
        public const int NoofTables = 10;
        public const int NoofdgitsInContactNumber = 10;
        private const int MaximumAdvanceReservationMonths = 3;
        public List<Reservation> reservations;
        public List<int> restaurantTableNumbers;
        private string reservationsFilePath = FileManager.ReservationsFilePath;

        public string LoadMessage { get; private set; } = string.Empty;
        public int ReservationWindowDurationHours
        {
            get
            {
                return ReservationWindowHours;
            }
        }

        public ReservationManager()
        {
            reservations = new List<Reservation>();
            restaurantTableNumbers = new List<int>();
            for (int tableNumber = 1; tableNumber <= NoofTables; tableNumber++)
            {
                restaurantTableNumbers.Add(tableNumber);
            }
            reservations = Load();
        }

        public int GetNextReservationId()
        {
            return reservations.Count + 1;
        }

        public List<Reservation> GetAllReservations()
        {
            return new List<Reservation>(reservations);
        }

        public bool AddReservation(Reservation reservation, out string message)
        {
            if (!ValidateReservation(reservation, out message))
            {
                return false;
            }
            if (!IsTableAvailable(reservation.TableNumber, reservation.ReservationDateTime))
            {
                message = "This table is already reserved for the selected time window.";
                return false;
            }
            reservations.Add(reservation);
            if (!Save(reservations))
            {
                message = GetStorageErrorMessage("Reservation created, but reservation data could not be saved.");
                return false;
            }
            message = "Reservation created successfully.";
            return true;
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
                bool reservationIsBooked = reservation.Status == ReservationStatus.Booked;
                bool reservationTimeOverlaps = requestedStartTime < reservation.ReservationDateTime.AddHours(ReservationWindowHours) &&
                                               reservation.ReservationDateTime < requestedEndTime;
                if (sameTable && reservationIsBooked && reservationTimeOverlaps)
                {
                    return false;
                }
            }
            return true;
        }

        public List<int> GetAvailableTables(DateTime reservationDateTime)
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

        public bool CancelReservation(int reservationId, out string message)
        {
            Reservation? reservation = SearchReservationById(reservationId);
            if (reservation == null)
            {
                message = "Reservation not found.";
                return false;
            }
            reservation.Status = ReservationStatus.Cancelled;
            if (!Save(reservations))
            {
                message = GetStorageErrorMessage("Reservation cancelled, but reservation data could not be saved.");
                return false;
            }
            message = "Reservation cancelled successfully.";
            return true;
        }

        private bool ValidateReservation(Reservation reservation, out string message)
        {
            if (reservation == null)
            {
                message = "Reservation cannot be null.";
                return false;
            }
            if (!IsValidCustomerName(reservation.CustomerName))
            {
                message = "Customer name should contain alphabets only.";
                return false;
            }
            if (!IsValidContactNumber(reservation.ContactNumber))
            {
                message = "Contact number must contain exactly 10 digits.";
                return false;
            }
            if (reservation.TableNumber <= 0 || reservation.TableNumber > NoofTables)
            {
                message = "Table number must be between 1 and 10.";
                return false;
            }
            if (reservation.NumberOfGuests <= 0)
            {
                message = "Number of guests must be greater than zero.";
                return false;
            }
            if (!IsValidReservationDateTime(reservation.ReservationDateTime))
            {
                message = "Reservation date and time cannot be in the past or more than 3 months from now.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private bool IsValidCustomerName(string? customerName)
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

        private bool IsValidContactNumber(string? contactNumber)
        {
            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                return false;
            }
            string trimmedContactNumber = contactNumber.Trim();
            if (trimmedContactNumber.Length != NoofdgitsInContactNumber)
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

        public bool Save(List<Reservation> items)
        {
            List<string> reservationLines = new List<string>();
            foreach (Reservation reservation in items)
            {
                string customerName = (reservation.CustomerName ?? "").Replace("|", " ");
                string contactNumber = (reservation.ContactNumber ?? "").Replace("|", " ");
                string reservationStatus = reservation.Status.ToString();
                reservationLines.Add(
                    $"{reservation.ReservationId}|{customerName}|{contactNumber}|{reservation.TableNumber}|{reservation.NumberOfGuests}|{reservation.ReservationDateTime:O}|{reservationStatus}");
            }
            return FileManager.TryWriteAllLines(reservationsFilePath, reservationLines);
        }

        public List<Reservation> Load()
        {
            LoadMessage = string.Empty;
            if (!FileManager.TryReadAllLines(reservationsFilePath, out string[] reservationLines))
            {
                LoadMessage = FileManager.LastErrorMessage;
                return new List<Reservation>();
            }
            try
            {
                List<Reservation> loadedReservations = ParseReservations(reservationLines);
                reservations = loadedReservations;
                return loadedReservations;
            }
            catch (Exception ex)
            {
                LoadMessage = ExceptionUtilities.GetMessage(
                    new ReservationException("Unexpected error while loading reservations.", ex));
                return new List<Reservation>();
            }
        }

        private List<Reservation> ParseReservations(string[] reservationLines)
        {
            List<Reservation> loadedReservations = new List<Reservation>();
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
                bool dateTimeValid = DateTime.TryParseExact(
                    reservationData[5],
                    "O",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTime reservationDateTime);
                if (!reservationIdValid || !tableNumberValid || !guestCountValid || !dateTimeValid)
                {
                    continue;
                }
                ReservationStatus reservationStatus;
                if (!Enum.TryParse(reservationData[6].Trim(), out reservationStatus))
                {
                    reservationStatus = ReservationStatus.Booked;
                }
                string customerName = reservationData[1].Trim();
                string contactNumber = reservationData[2].Trim();
                Reservation reservation = new Reservation(
                    reservationId,
                    customerName,
                    contactNumber,
                    tableNumber,
                    numberOfGuests,
                    reservationDateTime,
                    reservationStatus);
                loadedReservations.Add(reservation);
            }
            return loadedReservations;
        }

        private static string GetStorageErrorMessage(string fallbackMessage)
        {
            if (!string.IsNullOrWhiteSpace(FileManager.LastErrorMessage))
            {
                return FileManager.LastErrorMessage;
            }
            return fallbackMessage;
        }
    }
}
