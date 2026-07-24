using GourmetSpot.Models;
using GourmetSpot.Services;

namespace GourmetSpot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InventoryManager inventoryManager = new InventoryManager();
            MenuManager menuManager = new MenuManager();
            OrderManager orderManager = new OrderManager();
            BillManager billManager = new BillManager();

            while (true)
            {
                DisplayMainMenu();

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        InventorySubMenu(inventoryManager);
                        break;
                    case "2":
                        MenuSubMenu(menuManager);
                        break;
                    case "3":
                        OrderSubMenu(orderManager, menuManager, inventoryManager, billManager);
                        break;
                    case "4":
                        Console.WriteLine("Thank you for using the Restaurant Management System.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("====== The Gourmet Spot Restaurant Management ======");
            Console.WriteLine();
            Console.WriteLine("1. Inventory Management");
            Console.WriteLine("2. Menu Management");
            Console.WriteLine("3. Order Management");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
        }

        private static void InventorySubMenu(InventoryManager inventoryManager)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("===== Inventory Menu =====");
                Console.WriteLine("1. Add Ingredient");
                Console.WriteLine("2. View Ingredient");
                Console.WriteLine("3. Search Ingredient");
                Console.WriteLine("4. Update Ingredient");
                Console.WriteLine("5. Delete Ingredient");
                Console.WriteLine("6. Back");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AddIngredient(inventoryManager);
                        break;
                    case "2":
                        ViewInventory(inventoryManager);
                        break;
                    case "3":
                        SearchIngredient(inventoryManager);
                        break;
                    case "4":
                        UpdateIngredient(inventoryManager);
                        break;
                    case "5":
                        DeleteIngredient(inventoryManager);
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void MenuSubMenu(MenuManager menuManager)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("===== Menu Management =====");
                Console.WriteLine("1. Add Menu Item");
                Console.WriteLine("2. View Menu");
                Console.WriteLine("3. Search Menu Item");
                Console.WriteLine("4. Back");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AddMenuItem(menuManager);
                        break;
                    case "2":
                        ViewMenu(menuManager);
                        break;
                    case "3":
                        SearchMenuItem(menuManager);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void OrderSubMenu(
            OrderManager orderManager,
            MenuManager menuManager,
            InventoryManager inventoryManager,
            BillManager billManager)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("===== Order Management =====");
                Console.WriteLine("1. Create Order");
                Console.WriteLine("2. View Orders");
                Console.WriteLine("3. Back");
                Console.Write("Enter Choice : ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        CreateOrder(orderManager, menuManager, inventoryManager, billManager);
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

        private static void CreateOrder(
            OrderManager orderManager,
            MenuManager menuManager,
            InventoryManager inventoryManager,
            BillManager billManager)
        {
            try
            {
                Console.Write("Enter Order ID : ");
                int orderId = Convert.ToInt32(Console.ReadLine());

                Order order = new Order(orderId);

                Console.Write("How many menu items? ");
                int count = Convert.ToInt32(Console.ReadLine());

                bool orderValid = true;
                List<(MenuItem MenuItem, int Quantity)> selectedItems = new();

                for (int i = 0; i < count; i++)
                {
                    Console.Write("Enter Menu Item ID : ");
                    int menuId = Convert.ToInt32(Console.ReadLine());

                    MenuItem? menuItem = menuManager.SearchMenuItem(menuId);

                    if (menuItem == null)
                    {
                        Console.WriteLine("Menu Item Not Found.");
                        orderValid = false;
                        break;
                    }

                    Console.Write("Enter Quantity : ");
                    int quantity = Convert.ToInt32(Console.ReadLine());

                    bool stockAvailable = inventoryManager.HasEnoughIngredients(menuItem.Recipe, quantity);

                    if (!stockAvailable)
                    {
                        Console.WriteLine("Order cannot be placed.");
                        orderValid = false;
                        break;
                    }

                    selectedItems.Add((menuItem, quantity));
                }

                if (!orderValid || selectedItems.Count == 0)
                {
                    Console.WriteLine("Order was not placed.");
                    return;
                }

                foreach (var selectedItem in selectedItems)
                {
                    bool stockAvailable = inventoryManager.UseIngredients(selectedItem.MenuItem.Recipe, selectedItem.Quantity);

                    if (!stockAvailable)
                    {
                        Console.WriteLine("Order cannot be placed.");
                        return;
                    }

                    OrderItem orderItem = new OrderItem(selectedItem.MenuItem, selectedItem.Quantity);
                    order.AddItem(orderItem);
                }

                orderManager.CreateOrder(order);
                billManager.GenerateBill(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AddIngredient(InventoryManager inventoryManager)
        {
            try
            {
                Console.Write("Enter Ingredient ID: ");
                int id = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter Ingredient Name: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Enter Quantity: ");
                double quantity = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter Unit: ");
                string unit = Console.ReadLine() ?? "";

                Ingredient ingredient = new Ingredient(id, name, quantity, unit);

                inventoryManager.AddIngredient(ingredient);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter valid numeric values.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        private static void ViewInventory(InventoryManager inventoryManager)
        {
            inventoryManager.DisplayInventory();
        }

        private static void SearchIngredient(InventoryManager inventoryManager)
        {
            try
            {
                Console.Write("Enter Ingredient ID: ");
                int id = Convert.ToInt32(Console.ReadLine());

                Ingredient? ingredient = inventoryManager.SearchIngredientById(id);

                if (ingredient != null)
                {
                    Console.WriteLine("\nIngredient Found");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine(ingredient);
                }
                else
                {
                    Console.WriteLine("Ingredient not found.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a valid numeric ID.");
            }
        }

        private static void UpdateIngredient(InventoryManager inventoryManager)
        {
            try
            {
                Console.Write("Enter Ingredient ID: ");
                int id = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter New Quantity: ");
                double quantity = Convert.ToDouble(Console.ReadLine());

                bool updated = inventoryManager.UpdateIngredientQuantity(id, quantity);

                if (updated)
                {
                    Console.WriteLine("Ingredient updated successfully.");
                }
                else
                {
                    Console.WriteLine("Ingredient not found.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input.");
            }
        }

        private static void DeleteIngredient(InventoryManager inventoryManager)
        {
            try
            {
                Console.Write("Enter Ingredient ID: ");
                int id = Convert.ToInt32(Console.ReadLine());

                bool deleted = inventoryManager.DeleteIngredient(id);

                if (deleted)
                {
                    Console.WriteLine("Ingredient deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Ingredient not found.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input.");
            }
        }

       private static void AddMenuItem(MenuManager menuManager)
        {
            try
            {
                Console.Write("Enter Menu Item ID: ");
                int id = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter Menu Item Name: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Enter Price: ");
                decimal price = Convert.ToDecimal(Console.ReadLine());

                Dictionary<int, double> recipe = new Dictionary<int, double>();

                Console.Write("How many ingredients are required? ");
                int ingredientCount = Convert.ToInt32(Console.ReadLine());

                for (int i = 1; i <= ingredientCount; i++)
                {
                    Console.WriteLine($"\nIngredient {i}");

                    Console.Write("Ingredient ID: ");
                    int ingredientId = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Required Quantity: ");
                    double quantity = Convert.ToDouble(Console.ReadLine());

                    recipe.Add(ingredientId, quantity);
                }

                MenuItem item = new MenuItem(id, name, price, recipe);

                menuManager.AddMenuItem(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ViewMenu(MenuManager menuManager)
        {
            menuManager.DisplayMenu();
        }

        private static void SearchMenuItem(MenuManager menuManager)
        {
            try
            {
                Console.Write("Enter Menu Item ID: ");
                int id = Convert.ToInt32(Console.ReadLine());

                MenuItem? item = menuManager.SearchMenuItem(id);

                if (item != null)
                {
                    Console.WriteLine("\nMenu Item Found");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine(item);
                }
                else
                {
                    Console.WriteLine("Menu item not found.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a valid numeric ID.");
            }
        }
    }
}