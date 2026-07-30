using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class OrderScreen
    {
        private OrderManager orderManager;
        private MenuManager menuManager;
        private InventoryManager inventoryManager;
        private BillManager billManager;

        public OrderScreen(
            OrderManager orderManager,
            MenuManager menuManager,
            InventoryManager inventoryManager,
            BillManager billManager)
        {
            this.orderManager = orderManager;
            this.menuManager = menuManager;
            this.inventoryManager = inventoryManager;
            this.billManager = billManager;
        }

        public void Show()
        {
            while (true)
            {
                DisplayOrderMenu();
                string userChoice = ConsoleInput.ReadMenuChoice();
                switch (userChoice)
                {
                    case "1":
                        TakeOrder();
                        break;
                    case "2":
                        DisplayOrders();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            }
        }

        private void DisplayOrderMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== Order Management =====");
            Console.WriteLine("1. Create Order");
            Console.WriteLine("2. View Orders");
            Console.WriteLine("3. Back");
            Console.Write("Enter Choice : ");
        }

        private void TakeOrder()
        {
            if (menuManager.GetAllMenuItems().Count == 0)
            {
                Console.WriteLine("Please add menu items before creating an order.");
                return;
            }
            int orderId = orderManager.GetNextOrderId();
            Console.WriteLine($"Order ID: {orderId}");
            string customerName = ConsoleInput.ReadRequiredText("Enter Customer Name: ");
            Order order = new Order(orderId, customerName);
            List<(MenuItem MenuItem, int Quantity)> selectedMenuItems = ReadSelectedMenuItems();
            if (!orderManager.CreateOrder(order, selectedMenuItems, inventoryManager, out string orderMessage))
            {
                Console.WriteLine(orderMessage);
                return;
            }
            Console.WriteLine();
            Console.WriteLine(orderMessage);
            Bill bill = billManager.GenerateBill(order);
            if (!bill.IsSaved)
            {
                Console.WriteLine(bill.SaveMessage);
                return;
            }
            Console.WriteLine($"Order Subtotal : ₹{bill.Subtotal}");
            DisplayBill(bill);
        }

        private List<(MenuItem MenuItem, int Quantity)> ReadSelectedMenuItems()
        {
            List<(MenuItem MenuItem, int Quantity)> selectedMenuItems = new();
            DisplayMenu();
            while (true)
            {
                int menuItemId = ConsoleInput.ReadInt("Enter Menu Item ID (0 to finish): ");
                if (menuItemId == 0)
                {
                    break;
                }
                MenuItem? menuItem = menuManager.SearchMenuItemById(menuItemId);
                if (menuItem == null)
                {
                    Console.WriteLine("Menu Item Not Found.");
                    continue;
                }
                int quantity = ConsoleInput.ReadPositiveInt("Enter Quantity: ");
                selectedMenuItems.Add((menuItem, quantity));
                Console.WriteLine($"{menuItem.Name} added to order.");
            }
            return selectedMenuItems;
        }

        private void DisplayOrders()
        {
            List<Order> orders = orderManager.GetAllOrders();
            if (orders.Count == 0)
            {
                Console.WriteLine("No Orders Found.");
                return;
            }
            foreach (Order order in orders)
            {
                DisplayOrder(order);
            }
        }

        private void DisplayOrder(Order order)
        {
            Bill bill = billManager.CreateBill(order);
            Console.WriteLine($"\nOrder ID : {order.OrderId}");
            Console.WriteLine($"Customer Name : {order.CustomerName}");
            foreach (OrderItem orderItem in bill.Items)
            {
                Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{billManager.CalculateItemTotalPrice(orderItem)}");
            }
            Console.WriteLine($"Order Subtotal : ₹{bill.Subtotal}");
        }

        private void DisplayMenu()
        {
            List<MenuItem> menuItems = menuManager.GetAllMenuItems();
            if (menuItems.Count == 0)
            {
                Console.WriteLine("Menu is Empty.");
                return;
            }
            Console.WriteLine("\n========== MENU ==========");
            foreach (MenuItem menuItem in menuItems)
            {
                Console.WriteLine($"{menuItem.MenuItemId} - {menuItem.Name} - ₹{menuItem.Price}");
                if (menuItem.Recipe.Count > 0)
                {
                    Console.WriteLine("Recipe:");
                    foreach (var recipeIngredient in menuItem.Recipe)
                    {
                        Console.WriteLine($"Ingredient ID : {recipeIngredient.Key}  Quantity : {recipeIngredient.Value}");
                    }
                }
                Console.WriteLine("--------------------------------");
            }
        }

        private void DisplayBill(Bill bill)
        {
            Console.WriteLine();
            Console.WriteLine("========== BILL ==========");
            Console.WriteLine($"Order ID : {bill.OrderId}");
            Console.WriteLine($"Customer Name : {bill.CustomerName}");
            Console.WriteLine("--------------------------");
            foreach (OrderItem orderItem in bill.Items)
            {
                Console.WriteLine($"{orderItem.MenuItem.Name} x {orderItem.Quantity} = ₹{billManager.CalculateItemTotalPrice(orderItem)}");
            }
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Subtotal : ₹{bill.Subtotal}");
            Console.WriteLine($"GST (18%): ₹{bill.Tax}");
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Grand Total : ₹{bill.GrandTotal}");
            Console.WriteLine(bill.SaveMessage);
        }
    }
}
