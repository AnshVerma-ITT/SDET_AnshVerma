using GourmetSpot.Models;
using GourmetSpot.Services;

namespace GourmetSpot.UserInterface
{
    public class MenuManagementConsoleMenu
    {
        private readonly MenuManager menuManager;
        private readonly InventoryManager inventoryManager;

        public MenuManagementConsoleMenu(MenuManager menuManager, InventoryManager inventoryManager)
        {
            this.menuManager = menuManager;
            this.inventoryManager = inventoryManager;
        }

        public void Show()
        {
            while (true)
            {
                DisplayMenuOptions();

                string userChoice = ConsoleInput.ReadMenuChoice();

                switch (userChoice)
                {
                    case "1":
                        AddMenuItem();
                        break;
                    case "2":
                        menuManager.DisplayMenu();
                        break;
                    case "3":
                        SearchMenuItemByName();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void DisplayMenuOptions()
        {
            Console.WriteLine();
            Console.WriteLine("===== Menu Management =====");
            Console.WriteLine("1. Add Menu Item");
            Console.WriteLine("2. View Menu");
            Console.WriteLine("3. Search Menu Item by Name");
            Console.WriteLine("4. Back");
            Console.Write("Enter your choice: ");
        }

        private void AddMenuItem()
        {
            try
            {
                int menuItemId = menuManager.GetNextMenuItemId();
                Console.WriteLine($"Menu Item ID: {menuItemId}");

                string menuItemName = ConsoleInput.ReadRequiredText("Enter Menu Item Name: ");
                decimal menuItemPrice = ConsoleInput.ReadPositiveDecimal("Enter Price: ");
                Dictionary<int, double> recipeIngredients = ReadRecipeIngredients();

                MenuItem menuItem = new MenuItem(
                    menuItemId,
                    menuItemName,
                    menuItemPrice,
                    recipeIngredients);

                menuManager.AddMenuItem(menuItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Dictionary<int, double> ReadRecipeIngredients()
        {
            Dictionary<int, double> recipeIngredients = new Dictionary<int, double>();
            int ingredientCount = ConsoleInput.ReadNonNegativeInt("How many ingredients are required? ");

            if (ingredientCount > 0)
            {
                inventoryManager.DisplayInventory();
            }

            for (int ingredientNumber = 1; ingredientNumber <= ingredientCount; ingredientNumber++)
            {
                Console.WriteLine($"\nIngredient {ingredientNumber}");

                int ingredientId = ReadExistingIngredientId();
                double requiredQuantity = ConsoleInput.ReadPositiveDouble("Required Quantity: ");

                if (recipeIngredients.ContainsKey(ingredientId))
                {
                    recipeIngredients[ingredientId] += requiredQuantity;
                }
                else
                {
                    recipeIngredients.Add(ingredientId, requiredQuantity);
                }
            }

            return recipeIngredients;
        }

        private int ReadExistingIngredientId()
        {
            while (true)
            {
                int ingredientId = ConsoleInput.ReadPositiveInt("Ingredient ID: ");

                if (inventoryManager.SearchIngredientById(ingredientId) != null)
                {
                    return ingredientId;
                }

                Console.WriteLine("Ingredient not found. Please choose an ingredient from inventory.");
            }
        }

        private void SearchMenuItemByName()
        {
            string menuItemName = ConsoleInput.ReadRequiredText("Enter Menu Item Name: ");
            MenuItem? menuItem = menuManager.SearchMenuItemByName(menuItemName);

            if (menuItem != null)
            {
                Console.WriteLine("\nMenu Item Found");
                Console.WriteLine("-------------------------");
                Console.WriteLine($"{menuItem.MenuItemId} - {menuItem.Name} - ₹{menuItem.Price}");
            }
            else
            {
                Console.WriteLine("Menu item not found.");
            }
        }
    }
}
