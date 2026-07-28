using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class OrderManager
    {
        private readonly List<Order> orders;
        private readonly string ordersFilePath = ApplicationStorage.OrdersFilePath;

        public OrderManager()
        {
            orders = new List<Order>();
            LoadOrders();
        }

        public int GetNextOrderId()
        {
            int nextOrderId = 1;

            foreach (Order order in orders)
            {
                if (order.OrderId >= nextOrderId)
                {
                    nextOrderId = order.OrderId + 1;
                }
            }

            return nextOrderId;
        }

        public void PlaceOrder(Order order)
        {
            orders.Add(order);
            SaveOrders();

            Console.WriteLine("\nOrder Placed Successfully.");
            Console.WriteLine($"Order Subtotal : ₹{order.CalculateSubtotal()}");
        }

        public void DisplayOrders()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No Orders Found.");
                return;
            }

            foreach (Order order in orders)
            {
                Console.WriteLine($"\nOrder ID : {order.OrderId}");
                Console.WriteLine($"Customer Name : {order.CustomerName}");

                foreach (OrderItem orderItem in order.Items)
                {
                    Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{orderItem.TotalPrice}");
                }

                Console.WriteLine($"Order Subtotal : ₹{order.CalculateSubtotal()}");
            }
        }

        private void SaveOrders()
        {
            List<string> orderLines = new List<string>();

            foreach (Order order in orders)
            {
                List<string> savedOrderItems = new List<string>();
                string customerName = order.CustomerName.Replace("|", " ");

                foreach (OrderItem orderItem in order.Items)
                {
                    savedOrderItems.Add($"{orderItem.MenuItem.MenuItemId}:{orderItem.MenuItem.Name}:{orderItem.MenuItem.Price}:{orderItem.Quantity}");
                }

                orderLines.Add($"{order.OrderId}|{customerName}|{string.Join(";", savedOrderItems)}");
            }

            ApplicationStorage.TryWriteAllLines(ordersFilePath, orderLines);
        }

        private void LoadOrders()
        {
            if (!ApplicationStorage.TryReadAllLines(ordersFilePath, out string[] orderLines))
            {
                return;
            }

            foreach (string orderLine in orderLines)
            {
                if (string.IsNullOrWhiteSpace(orderLine))
                {
                    continue;
                }

                string[] orderParts = orderLine.Split('|', 3);

                if (orderParts.Length < 2)
                {
                    continue;
                }

                bool orderIdValid = int.TryParse(orderParts[0], out int orderId);

                if (!orderIdValid)
                {
                    continue;
                }

                string customerName = "Walk-in Customer";
                string savedMenuItems;

                if (orderParts.Length == 2)
                {
                    savedMenuItems = orderParts[1];
                }
                else
                {
                    customerName = orderParts[1];
                    savedMenuItems = orderParts[2];
                }

                Order order = new Order(orderId, customerName);

                foreach (string savedMenuItem in savedMenuItems.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] savedMenuItemData = savedMenuItem.Split(':');

                    if (savedMenuItemData.Length < 4)
                    {
                        continue;
                    }

                    bool menuItemIdValid = int.TryParse(savedMenuItemData[0], out int menuItemId);
                    bool menuItemPriceValid = decimal.TryParse(savedMenuItemData[2], out decimal menuItemPrice);
                    bool menuItemQuantityValid = int.TryParse(savedMenuItemData[3], out int menuItemQuantity);

                    if (!menuItemIdValid || !menuItemPriceValid || !menuItemQuantityValid)
                    {
                        continue;
                    }

                    MenuItem menuItem = new MenuItem(
                        menuItemId,
                        savedMenuItemData[1],
                        menuItemPrice,
                        new Dictionary<int, double>());

                    OrderItem orderItem = new OrderItem(menuItem, menuItemQuantity);
                    order.AddItem(orderItem);
                }

                orders.Add(order);
            }
        }
    }
}
