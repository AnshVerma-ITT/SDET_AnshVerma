using System.Text.Json;
using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class MenuManager
    {
        private readonly List<MenuItem> menuItems;
        private readonly string menuFilePath = ApplicationStorage.MenuFilePath;

        public MenuManager()
        {
            menuItems = new List<MenuItem>();
            LoadMenu();
        }

        public int GetNextMenuItemId()
        {
            int nextMenuItemId = 1;

            foreach (MenuItem menuItem in menuItems)
            {
                if (menuItem.MenuItemId >= nextMenuItemId)
                {
                    nextMenuItemId = menuItem.MenuItemId + 1;
                }
            }

            return nextMenuItemId;
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            menuItems.Add(menuItem);
            SaveMenu();
            Console.WriteLine("Menu Item Added Successfully.");
        }

        public void DisplayMenu()
        {
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

        public MenuItem? SearchMenuItemById(int menuItemId)
        {
            foreach (MenuItem menuItem in menuItems)
            {
                if (menuItem.MenuItemId == menuItemId)
                {
                    return menuItem;
                }
            }

            return null;
        }

        public MenuItem? SearchMenuItemByName(string menuItemName)
        {
            foreach (MenuItem menuItem in menuItems)
            {
                if (menuItem.Name.Trim().Equals(menuItemName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return menuItem;
                }
            }

            return null;
        }

        public List<MenuItem> GetAllMenuItems()
        {
            return menuItems;
        }

        private void SaveMenu()
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string menuJson = JsonSerializer.Serialize(menuItems, jsonOptions);
            ApplicationStorage.TryWriteAllText(menuFilePath, menuJson);
        }

        private void LoadMenu()
        {
            if (!ApplicationStorage.TryReadAllText(menuFilePath, out string menuJson))
            {
                return;
            }

            try
            {
                List<MenuItem>? savedMenuItems = JsonSerializer.Deserialize<List<MenuItem>>(menuJson);

                if (savedMenuItems != null)
                {
                    menuItems.AddRange(savedMenuItems);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Unable to read menu data from '{menuFilePath}': {ex.Message}");
            }
        }
    }
}
