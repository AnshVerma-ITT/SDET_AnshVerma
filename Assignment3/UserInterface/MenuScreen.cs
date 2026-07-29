using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class MenuScreen
    {
        private MenuManager menuManager;
        private InventoryManager inventoryManager;

        public MenuScreen(MenuManager menuManager, InventoryManager inventoryManager)
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
                        DisplayMenu();
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
            menuManager.AddMenuItem(menuItem, out string addMessage);
            Console.WriteLine(addMessage);
        }

        private Dictionary<int, double> ReadRecipeIngredients()
        {
            Dictionary<int, double> recipeIngredients = menuManager.CreateRecipe();
            int ingredientCount = ConsoleInput.ReadNonNegativeInt("How many ingredients are required? ");
            if (ingredientCount > 0)
            {
                DisplayInventory();
            }
            for (int ingredientNumber = 1; ingredientNumber <= ingredientCount; ingredientNumber++)
            {
                Console.WriteLine($"\nIngredient {ingredientNumber}");
                int ingredientId = ReadExistingIngredientId();
                double requiredQuantity = ConsoleInput.ReadPositiveDouble("Required Quantity: ");
                menuManager.AddRecipeIngredient(recipeIngredients, ingredientId, requiredQuantity);
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

        private void DisplayInventory()
        {
            List<Ingredient> ingredients = inventoryManager.GetAllIngredients();
            if (ingredients.Count == 0)
            {
                Console.WriteLine("\nInventory is empty.");
                return;
            }
            Console.WriteLine("\n========== INVENTORY ==========");
            foreach (Ingredient ingredient in ingredients)
            {
                Console.WriteLine($"{ingredient.IngredientId} - {ingredient.Name} - {ingredient.Quantity} {ingredient.Unit}");
            }
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
                DisplayMenuItem(menuItem, true);
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
                DisplayMenuItem(menuItem, false);
            }
            else
            {
                Console.WriteLine("Menu item not found.");
            }
        }

        private void DisplayMenuItem(MenuItem menuItem, bool includeRecipe)
        {
            Console.WriteLine($"{menuItem.MenuItemId} - {menuItem.Name} - ₹{menuItem.Price}");
            if (includeRecipe && menuItem.Recipe.Count > 0)
            {
                Console.WriteLine("Recipe:");
                foreach (var recipeIngredient in menuItem.Recipe)
                {
                    Console.WriteLine($"Ingredient ID : {recipeIngredient.Key}  Quantity : {recipeIngredient.Value}");
                }
            }
            if (includeRecipe)
            {
                Console.WriteLine("--------------------------------");
            }
        }
    }
}
