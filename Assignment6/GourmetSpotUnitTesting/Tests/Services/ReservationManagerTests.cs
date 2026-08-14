using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class ReservationManagerTests : FileTestBase
    {
        [Test]
        public void AddReservation_WhenValid_AddsReservationAndBlocksTable()
        {
            ReservationManager manager = new ReservationManager();
            DateTime reservationTime = DateTime.Today.AddDays(7).AddHours(18);
            int tableNumber = 1;
            Reservation reservation = new Reservation(
                1,
                "Customer",
                "9876543210",
                tableNumber,
                2,
                reservationTime);
            bool added = manager.AddReservation(reservation, out string message);
            Assert.That(added, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(manager.SearchReservationById(reservation.ReservationId), Is.Not.Null);
            Assert.That(manager.IsTableAvailable(tableNumber, reservationTime), Is.False);
            Assert.That(
                manager.IsTableAvailable(
                    tableNumber,
                    reservationTime.AddHours(ReservationManager.ReservationWindowHours)),
                Is.True);
            Assert.That(manager.GetAvailableTables(reservationTime), Does.Not.Contain(tableNumber));
        }

        [Test]
        public void AddReservation_WhenTableAlreadyBooked_ReturnsFalse()
        {
            ReservationManager manager = new ReservationManager();
            DateTime reservationTime = DateTime.Today.AddDays(7).AddHours(18);
            int tableNumber = 1;
            Reservation firstReservation = new Reservation(
                1,
                "Customer",
                "9876543210",
                tableNumber,
                2,
                reservationTime);
            Reservation overlappingReservation = new Reservation(
                2,
                "Customer",
                "9876543210",
                tableNumber,
                2,
                reservationTime.AddHours(1));
            manager.AddReservation(firstReservation, out _);
            bool added = manager.AddReservation(
                overlappingReservation,
                out string message);
            Assert.That(added, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void AddReservation_WhenContactNumberIsInvalid_ReturnsFalse()
        {
            ReservationManager manager = new ReservationManager();
            Reservation reservation = new Reservation(
                1,
                "Customer",
                "12345",
                1,
                2,
                DateTime.Today.AddDays(7).AddHours(18));
            bool added = manager.AddReservation(reservation, out string message);
            Assert.That(added, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void CancelReservation_WhenReservationExists_CancelsReservation()
        {
            ReservationManager manager = new ReservationManager();
            DateTime reservationTime = DateTime.Today.AddDays(7).AddHours(18);
            int tableNumber = 1;
            Reservation reservation = new Reservation(
                1,
                "Customer",
                "9876543210",
                tableNumber,
                2,
                reservationTime);
            manager.AddReservation(reservation, out _);
            bool cancelled = manager.CancelReservation(reservation.ReservationId, out string message);
            Assert.That(cancelled, Is.True);
            Assert.That(message, Is.Not.Empty);
            Assert.That(
                manager.SearchReservationById(reservation.ReservationId)!.Status,
                Is.EqualTo(ReservationStatus.Cancelled));
            Assert.That(manager.IsTableAvailable(tableNumber, reservationTime), Is.True);
        }

        [Test]
        public void CancelReservation_WhenReservationDoesNotExist_ReturnsFalse()
        {
            ReservationManager manager = new ReservationManager();
            bool cancelled = manager.CancelReservation(1, out string message);
            Assert.That(cancelled, Is.False);
            Assert.That(message, Is.Not.Empty);
        }
    }
}
