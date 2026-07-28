using GourmetSpot.Models;
using GourmetSpot.Services;

namespace GourmetSpot.UserInterface
{
    public class OrderConsoleMenu
    {
        private readonly OrderManager orderManager;
        private readonly MenuManager menuManager;
        private readonly InventoryManager inventoryManager;
        private readonly BillManager billManager;

        public OrderConsoleMenu(
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
                        orderManager.DisplayOrders();
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
            try
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

                if (selectedMenuItems.Count == 0)
                {
                    Console.WriteLine("Order was not placed.");
                    return;
                }

                Dictionary<int, double> requiredIngredients = CalculateRequiredIngredients(selectedMenuItems);

                if (!inventoryManager.UseIngredients(requiredIngredients))
                {
                    Console.WriteLine("Order cannot be placed.");
                    return;
                }

                AddItemsToOrder(order, selectedMenuItems);
                orderManager.PlaceOrder(order);
                billManager.GenerateBill(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<(MenuItem MenuItem, int Quantity)> ReadSelectedMenuItems()
        {
            List<(MenuItem MenuItem, int Quantity)> selectedMenuItems = new();

            menuManager.DisplayMenu();

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

        private Dictionary<int, double> CalculateRequiredIngredients(List<(MenuItem MenuItem, int Quantity)> selectedMenuItems)
        {
            Dictionary<int, double> requiredIngredients = new Dictionary<int, double>();

            foreach (var selectedMenuItem in selectedMenuItems)
            {
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
                order.AddItem(orderItem);
            }
        }
    }
}
