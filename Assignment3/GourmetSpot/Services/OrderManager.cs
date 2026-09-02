using GourmetSpot.Models;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class OrderManager
    {
        private List<Order> orders;
        private string ordersFilePath = FileManager.OrdersFilePath;

        public string LoadMessage { get; private set; } = string.Empty;

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

        public List<Order> GetAllOrders()
        {
            return new List<Order>(orders);
        }

        public bool CreateOrder(
            Order order,
            List<(MenuItem MenuItem, int Quantity)> selectedMenuItems,
            InventoryManager inventoryManager,
            out string message)
        {
            if (order == null)
            {
                message = "Order cannot be null.";
                return false;
            }
            if (selectedMenuItems == null || selectedMenuItems.Count == 0)
            {
                message = "Order was not placed.";
                return false;
            }
            if (inventoryManager == null)
            {
                message = "Inventory manager is not available.";
                return false;
            }
            Dictionary<int, double> requiredIngredients = CalculateRequiredIngredients(selectedMenuItems);
            if (!inventoryManager.UseIngredients(requiredIngredients, out string inventoryMessage))
            {
                if (string.IsNullOrWhiteSpace(inventoryMessage))
                {
                    message = "Order cannot be placed.";
                }
                else
                {
                    message = $"{inventoryMessage}\nOrder cannot be placed.";
                }
                return false;
            }
            AddItemsToOrder(order, selectedMenuItems);
            return PlaceOrder(order, out message);
        }

        private bool PlaceOrder(Order order, out string message)
        {
            orders.Add(order);
            if (!SaveOrders())
            {
                message = GetStorageErrorMessage("Order placed, but order data could not be saved.");
                return false;
            }
            message = "Order Placed Successfully.";
            return true;
        }

        private void AddItem(Order order, OrderItem orderItem)
        {
            order.Items.Add(orderItem);
        }

        private Dictionary<int, double> CalculateRequiredIngredients(List<(MenuItem MenuItem, int Quantity)> selectedMenuItems)
        {
            Dictionary<int, double> requiredIngredients = new Dictionary<int, double>();
            foreach (var selectedMenuItem in selectedMenuItems)
            {
                if (selectedMenuItem.MenuItem.Recipe == null)
                {
                    continue;
                }
                foreach (var recipeIngredient in selectedMenuItem.MenuItem.Recipe)
                {
                    double requiredQuantity = recipeIngredient.Value * selectedMenuItem.Quantity;
                    if (requiredIngredients.ContainsKey(recipeIngredient.Key))
                    {
                        requiredIngredients[recipeIngredient.Key] += requiredQuantity;
                    }
                    else
                    {
                        requiredIngredients.Add(recipeIngredient.Key, requiredQuantity);
                    }
                }
            }
            return requiredIngredients;
        }

        private void AddItemsToOrder(Order order, List<(MenuItem MenuItem, int Quantity)> selectedMenuItems)
        {
            foreach (var selectedMenuItem in selectedMenuItems)
            {
                OrderItem orderItem = new OrderItem(selectedMenuItem.MenuItem, selectedMenuItem.Quantity);
                AddItem(order, orderItem);
            }
        }

        private bool SaveOrders()
        {
            List<string> orderLines = new List<string>();
            foreach (Order order in orders)
            {
                List<string> savedOrderItems = new List<string>();
                string customerName = (order.CustomerName ?? "Walk-in Customer").Replace("|", " ");
                foreach (OrderItem orderItem in order.Items)
                {
                    string menuItemName = (orderItem.MenuItem.Name ?? "Unnamed Item").Replace(":", " ");
                    savedOrderItems.Add($"{orderItem.MenuItem.MenuItemId}:{menuItemName}:{orderItem.MenuItem.Price}:{orderItem.Quantity}");
                }
                orderLines.Add($"{order.OrderId}|{customerName}|{string.Join(";", savedOrderItems)}");
            }
            return FileManager.TryWriteAllLines(ordersFilePath, orderLines);
        }

        private void LoadOrders()
        {
            LoadMessage = string.Empty;
            if (!FileManager.TryReadAllLines(ordersFilePath, out string[] orderLines))
            {
                LoadMessage = FileManager.LastErrorMessage;
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
                    AddItem(order, orderItem);
                }
                orders.Add(order);
            }
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
