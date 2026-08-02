using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IReservationManager
    {
        string LoadMessage { get; }
        int ReservationWindowDurationHours { get; }

        int GetNextReservationId();
        List<Reservation> GetAllReservations();
        bool AddReservation(Reservation reservation, out string message);
        bool HasAvailableTables(DateTime reservationDateTime);
        bool IsTableAvailable(int tableNumber, DateTime reservationDateTime);
        List<int> GetAvailableTables(DateTime reservationDateTime);
        Reservation? SearchReservationById(int reservationId);
        bool CancelReservation(int reservationId, out string message);
    }
}
