using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Tests.Helpers;

namespace GourmetSpot.Tests.Services
{
    public class ReservationManagerTests : ManagerTestBase<ReservationManager>
    {
        [Test]
        public void GetNextReservationId_WhenReservationsExist_ReturnsCountPlusOne()
        {
            List<Reservation> savedReservations = new List<Reservation>
            {
                TestData.CreateReservation()
            };
            SetField(
                "reservations",
                savedReservations);
            int nextId = Manager.GetNextReservationId();
            Assert.That(nextId, Is.EqualTo(savedReservations.Count + TestData.FirstId), "GetNextReservationId should return current reservation count plus one.");
        }

        [Test]
        public void GetAllReservations_WhenReservationsExist_ReturnsCopy()
        {
            Reservation reservation = TestData.CreateReservation();
            List<Reservation> savedReservations = new List<Reservation> { reservation };
            SetField(
                "reservations",
                savedReservations);
            List<Reservation> reservations = Manager.GetAllReservations();
            Assert.That(reservations, Is.Not.SameAs(savedReservations), "GetAllReservations should return a new list instead of the internal reservation list.");
            Assert.That(reservations, Has.Count.EqualTo(savedReservations.Count), "GetAllReservations should include all saved reservations.");
            Assert.That(reservations[TestData.FirstIndex], Is.SameAs(reservation), "GetAllReservations should return the saved reservation.");
        }

        [Test]
        public void HasAvailableTables_WhenAtLeastOneTableIsFree_ReturnsTrue()
        {
            SetField(
                "reservations",
                new List<Reservation>());
            SetField(
                "restaurantTableNumbers",
                new List<int> { TestData.TableNumber });
            bool hasAvailableTables = Manager.HasAvailableTables(TestData.ReservationTime);
            Assert.That(hasAvailableTables, Is.True, "HasAvailableTables should return true when at least one restaurant table is free.");
        }

        [Test]
        public void IsTableAvailable_WhenBookingOverlaps_ReturnsFalse()
        {
            DateTime reservationTime = TestData.ReservationTime;
            int tableNumber = TestData.TableNumber;
            Reservation reservation = TestData.CreateReservation(
                tableNumber: tableNumber,
                reservationDateTime: reservationTime);
            SetField(
                "reservations",
                new List<Reservation> { reservation });
            SetField(
                "restaurantTableNumbers",
                new List<int> { tableNumber });
            bool available = Manager.IsTableAvailable(
                tableNumber,
                reservationTime.AddHours(TestData.SingleQuantity));
            Assert.That(available, Is.False, "IsTableAvailable should return false when the requested time overlaps an existing booking.");
        }

        [Test]
        public void IsTableAvailable_WhenBookingDoesNotOverlap_ReturnsTrue()
        {
            DateTime reservationTime = TestData.ReservationTime;
            int tableNumber = TestData.TableNumber;
            Reservation reservation = TestData.CreateReservation(
                tableNumber: tableNumber,
                reservationDateTime: reservationTime);
            SetField(
                "reservations",
                new List<Reservation> { reservation });
            SetField(
                "restaurantTableNumbers",
                new List<int> { tableNumber });
            bool available = Manager.IsTableAvailable(
                tableNumber,
                reservationTime.AddHours(ReservationManager.ReservationWindowHours));
            Assert.That(available, Is.True, "IsTableAvailable should return true when the requested time starts after the reservation window.");
        }

        [Test]
        public void GetAvailableTables_WhenOneTableIsReserved_ReturnsOnlyFreeTables()
        {
            DateTime reservationTime = TestData.ReservationTime;
            Reservation reservation = TestData.CreateReservation(
                tableNumber: TestData.TableNumber,
                reservationDateTime: reservationTime);
            SetField(
                "reservations",
                new List<Reservation> { reservation });
            SetField(
                "restaurantTableNumbers",
                new List<int> { TestData.TableNumber, TestData.OtherTableNumber });
            List<int> availableTables = Manager.GetAvailableTables(reservationTime);
            Assert.That(availableTables, Is.EqualTo(new List<int> { TestData.OtherTableNumber }), "GetAvailableTables should return only tables not reserved for the requested time.");
        }

        [Test]
        public void SearchReservationById_WhenIdExists_ReturnsReservation()
        {
            Reservation reservation = TestData.CreateReservation();
            SetField(
                "reservations",
                new List<Reservation> { reservation });
            Reservation? foundReservation = Manager.SearchReservationById(reservation.ReservationId);
            Assert.That(foundReservation, Is.SameAs(reservation), "SearchReservationById should return the reservation with the matching id.");
        }

        [Test]
        public void AddReservation_WhenTableAlreadyBooked_ReturnsFalse()
        {
            DateTime reservationTime = TestData.ReservationTime;
            int tableNumber = TestData.TableNumber;
            Reservation firstReservation = TestData.CreateReservation(
                tableNumber: tableNumber,
                reservationDateTime: reservationTime);
            Reservation overlappingReservation = TestData.CreateReservation(
                reservationId: TestData.SecondId,
                tableNumber: tableNumber,
                reservationDateTime: reservationTime.AddHours(TestData.SingleQuantity));
            SetField(
                "reservations",
                new List<Reservation> { firstReservation });
            SetField(
                "restaurantTableNumbers",
                new List<int> { tableNumber });
            bool added = Manager.AddReservation(
                overlappingReservation,
                out string message);
            Assert.That(added, Is.False, "AddReservation should reject a reservation that overlaps an existing booking for the same table.");
            Assert.That(message, Is.Not.Empty, "AddReservation should return a validation message when the table is already booked.");
        }

        [Test]
        public void AddReservation_WhenContactNumberIsInvalid_ReturnsFalse()
        {
            Reservation reservation = TestData.CreateReservation(
                contactNumber: TestData.InvalidContactNumber);
            bool added = Manager.AddReservation(reservation, out string message);
            Assert.That(added, Is.False, "AddReservation should reject an invalid contact number.");
            Assert.That(message, Is.Not.Empty, "AddReservation should return a validation message for invalid contact number.");
        }

        [Test]
        public void CancelReservation_WhenReservationDoesNotExist_ReturnsFalse()
        {
            SetField(
                "reservations",
                new List<Reservation>());
            bool cancelled = Manager.CancelReservation(TestData.FirstId, out string message);
            Assert.That(cancelled, Is.False, "CancelReservation should return false when the reservation id does not exist.");
            Assert.That(message, Is.Not.Empty, "CancelReservation should return a message when the reservation is missing.");
        }
    }
}
