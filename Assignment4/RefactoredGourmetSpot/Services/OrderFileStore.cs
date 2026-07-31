using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class OrderFileStore : IOrderStore
    {
        private readonly IOrderCreator orderCreator;
        private readonly string ordersFilePath;

        public string LoadMessage { get; private set; } = string.Empty;

        public OrderFileStore(IOrderCreator orderCreator)
        {
            this.orderCreator = orderCreator;
            ordersFilePath = FileManager.OrdersFilePath;
        }

        public List<Order> LoadOrders()
        {
            List<Order> orders = new List<Order>();
            LoadMessage = string.Empty;
            if (!FileManager.TryReadAllLines(ordersFilePath, out string[] orderLines))
            {
                LoadMessage = FileManager.LastErrorMessage;
                return orders;
            }
            foreach (string orderLine in orderLines)
            {
                Order? order = DeserializeOrder(orderLine);
                if (order != null)
                {
                    orders.Add(order);
                }
            }
            return orders;
        }

        public bool SaveOrders(List<Order> orders)
        {
            List<string> orderLines = new List<string>();
            foreach (Order order in orders)
            {
                orderLines.Add(SerializeOrder(order));
            }
            return FileManager.TryWriteAllLines(ordersFilePath, orderLines);
        }

        private string SerializeOrder(Order order)
        {
            string customerName = SanitizeStoredText(order.CustomerName, "Walk-in Customer");
            string orderStatus = order.IsFinalized ? "Finalized" : "Open";
            string savedSubOrders = SerializeSubOrders(order);
            return $"{order.OrderId}|{customerName}|{order.OrderType}|{order.TableNumber}|{orderStatus}|{savedSubOrders}";
        }

        private string SerializeSubOrders(Order order)
        {
            List<string> savedSubOrders = new List<string>();
            foreach (SubOrder subOrder in order.SubOrders)
            {
                savedSubOrders.Add($"{subOrder.SubOrderNumber},{subOrder.OrderedAt:O},{SerializeOrderItems(subOrder.Items)}");
            }
            return string.Join("#", savedSubOrders);
        }

        private string SerializeOrderItems(IReadOnlyList<OrderItem> orderItems)
        {
            List<string> savedOrderItems = new List<string>();
            foreach (OrderItem orderItem in orderItems)
            {
                string menuItemName = SanitizeStoredText(orderItem.MenuItem.Name, "Unnamed Item");
                savedOrderItems.Add($"{orderItem.MenuItem.MenuItemId}:{menuItemName}:{orderItem.MenuItem.Price}:{orderItem.Quantity}");
            }
            return string.Join(";", savedOrderItems);
        }

        private Order? DeserializeOrder(string orderLine)
        {
            if (string.IsNullOrWhiteSpace(orderLine))
            {
                return null;
            }
            string[] orderParts = orderLine.Split('|', 6);
            if (orderParts.Length < 2)
            {
                return null;
            }
            if (!int.TryParse(orderParts[0], out int orderId))
            {
                return null;
            }
            if (orderParts.Length >= 6)
            {
                return DeserializeCurrentOrderFormat(orderParts, orderId);
            }
            if (orderParts.Length >= 5)
            {
                return DeserializePreviousSubOrderFormat(orderParts, orderId);
            }
            return DeserializeLegacyOrderFormat(orderParts, orderId);
        }

        private Order DeserializeCurrentOrderFormat(string[] orderParts, int orderId)
        {
            string customerName = orderParts[1];
            string orderType = orderParts[2];
            int tableNumber = ParseTableNumber(orderParts[3]);
            bool isFinalized = IsStoredOrderFinalized(orderParts[4]);
            Order order = orderCreator.CreateOrder(orderId, customerName, orderType, tableNumber, isFinalized);
            AddStoredSubOrders(order, orderParts[5]);
            return order;
        }

        private Order DeserializePreviousSubOrderFormat(string[] orderParts, int orderId)
        {
            string customerName = orderParts[1];
            int tableNumber = ParseTableNumber(orderParts[2]);
            string orderType = tableNumber > 0 ? OrderTypes.Table : OrderTypes.Customer;
            bool isFinalized = IsStoredOrderFinalized(orderParts[3]);
            Order order = orderCreator.CreateOrder(orderId, customerName, orderType, tableNumber, isFinalized);
            AddStoredSubOrders(order, orderParts[4]);
            return order;
        }

        private Order DeserializeLegacyOrderFormat(string[] orderParts, int orderId)
        {
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
            Order order = orderCreator.CreateOrder(
                orderId,
                customerName,
                OrderTypes.Customer,
                0,
                true);
            SubOrder subOrder = new SubOrder(1, DateTime.MinValue);
            foreach (OrderItem orderItem in DeserializeOrderItems(savedMenuItems))
            {
                subOrder.AddItem(orderItem);
            }
            if (subOrder.Items.Count > 0)
            {
                order.AddSubOrder(subOrder);
            }
            return order;
        }

        private void AddStoredSubOrders(Order order, string savedSubOrders)
        {
            foreach (string savedSubOrder in savedSubOrders.Split('#', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] savedSubOrderData = savedSubOrder.Split(',', 3);
                if (savedSubOrderData.Length < 3)
                {
                    continue;
                }
                if (!int.TryParse(savedSubOrderData[0], out int subOrderNumber))
                {
                    continue;
                }
                if (!DateTime.TryParse(savedSubOrderData[1], out DateTime orderedAt))
                {
                    orderedAt = DateTime.MinValue;
                }
                SubOrder subOrder = new SubOrder(subOrderNumber, orderedAt);
                foreach (OrderItem orderItem in DeserializeOrderItems(savedSubOrderData[2]))
                {
                    subOrder.AddItem(orderItem);
                }
                if (subOrder.Items.Count > 0)
                {
                    order.AddSubOrder(subOrder);
                }
            }
        }

        private List<OrderItem> DeserializeOrderItems(string savedMenuItems)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
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
                orderItems.Add(new OrderItem(menuItem, menuItemQuantity));
            }
            return orderItems;
        }

        private int ParseTableNumber(string value)
        {
            if (!int.TryParse(value, out int tableNumber))
            {
                return 0;
            }
            return tableNumber;
        }

        private bool IsStoredOrderFinalized(string value)
        {
            return !value.Equals("Open", StringComparison.OrdinalIgnoreCase);
        }

        private static string SanitizeStoredText(string? value, string fallbackValue)
        {
            return (value ?? fallbackValue)
                .Replace("|", " ")
                .Replace(":", " ")
                .Replace(";", " ")
                .Replace("#", " ")
                .Replace(",", " ");
        }
    }
}
