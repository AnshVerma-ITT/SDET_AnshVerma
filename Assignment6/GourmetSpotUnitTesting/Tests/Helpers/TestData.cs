using GourmetSpot.Models;

namespace GourmetSpot.Tests.Helpers
{
    internal static class TestData
    {
        public const int FirstId = 1;
        public const int SecondId = 2;
        public const int ThirdId = 3;
        public const int FirstIndex = 0;
        public const int TableNumber = 1;
        public const int OtherTableNumber = 2;
        public const int GuestCount = 2;
        public const int OrderQuantity = 2;
        public const int SingleQuantity = 1;
        public const int FutureReservationDays = 7;
        public const int ReservationHour = 18;
        public const decimal MenuItemPrice = 100;
        public const decimal SideItemPrice = 50;
        public const decimal GstRate = 0.18m;
        public const double StockQuantity = 10;
        public const double LowStockQuantity = 2;
        public const double RequiredQuantity = 2;
        public const double ExtraRequiredQuantity = 3;
        public const double SecondRequiredQuantity = 0.5;
        public const double NegativeQuantity = -1;
        public const decimal ZeroPrice = 0;
        public const string CustomerName = "Customer";
        public const string OtherCustomerName = "Another Customer";
        public const string IngredientName = "Ingredient";
        public const string MissingName = "Missing";
        public const string Unit = "kg";
        public const string MenuItemName = "Menu Item";
        public const string MainItemName = "Main Item";
        public const string SideItemName = "Side Item";
        public const string EmptyName = "";
        public const string WhiteSpaceName = " ";
        public const string ContactNumber = "9876543210";
        public const string InvalidContactNumber = "12345";
        public static DateTime OrderedAt => DateTime.UnixEpoch;
        public static DateTime ReservationTime => DateTime.Today
            .AddDays(FutureReservationDays)
            .AddHours(ReservationHour);

        public static Ingredient CreateIngredient(
            int ingredientId = FirstId,
            string name = IngredientName,
            double quantity = StockQuantity,
            string unit = Unit)
        {
            return new Ingredient(ingredientId, name, quantity, unit);
        }

        public static MenuItem CreateMenuItem(
            int menuItemId = FirstId,
            string name = MenuItemName,
            decimal price = MenuItemPrice,
            Dictionary<int, double>? recipe = null)
        {
            return new MenuItem(menuItemId, name, price, recipe ?? new Dictionary<int, double>());
        }

        public static MenuItem CreateMenuItemWithNullRecipe()
        {
            return new MenuItem(FirstId, MenuItemName, MenuItemPrice, null!);
        }

        public static OrderItem CreateOrderItem(
            MenuItem? menuItem = null,
            int quantity = OrderQuantity)
        {
            return new OrderItem(menuItem ?? CreateMenuItem(), quantity);
        }

        public static SubOrder CreateSubOrder(int subOrderNumber = FirstId)
        {
            return new SubOrder(subOrderNumber, OrderedAt);
        }

        public static TableOrder CreateTableOrder(
            int orderId = FirstId,
            string customerName = CustomerName,
            int tableNumber = TableNumber,
            bool isFinalized = false)
        {
            return new TableOrder(orderId, customerName, tableNumber, isFinalized);
        }

        public static TakeawayOrder CreateTakeawayOrder(
            int orderId = FirstId,
            string customerName = CustomerName,
            bool isFinalized = true)
        {
            return new TakeawayOrder(orderId, customerName, isFinalized);
        }

        public static Reservation CreateReservation(
            int reservationId = FirstId,
            string customerName = CustomerName,
            string contactNumber = ContactNumber,
            int tableNumber = TableNumber,
            int numberOfGuests = GuestCount,
            DateTime? reservationDateTime = null,
            ReservationStatus status = ReservationStatus.Booked)
        {
            return new Reservation(
                reservationId,
                customerName,
                contactNumber,
                tableNumber,
                numberOfGuests,
                reservationDateTime ?? ReservationTime,
                status);
        }

        public static Dictionary<int, double> CreateRecipe(
            int ingredientId = FirstId,
            double requiredQuantity = RequiredQuantity)
        {
            return new Dictionary<int, double>
            {
                { ingredientId, requiredQuantity }
            };
        }

        public static string DifferentCaseWithSpaces(string value)
        {
            return $" {value.ToLower()} ";
        }
    }
}
