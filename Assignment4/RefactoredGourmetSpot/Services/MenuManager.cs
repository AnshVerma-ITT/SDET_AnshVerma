using System.Text.Json;
using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class MenuManager : IMenuManager
    {
        private List<MenuItem> menuItems;
        private string menuFilePath = FileManager.MenuFilePath;
        private string storageErrorMessage = string.Empty;

        public string LoadMessage { get; private set; } = string.Empty;

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

        public bool AddMenuItem(MenuItem menuItem, out string message)
        {
            if (!ValidateMenuItem(menuItem, out message))
            {
                return false;
            }
            menuItems.Add(menuItem);
            if (!SaveMenu())
            {
                message = GetStorageErrorMessage("Menu item added, but menu could not be saved.");
                return false;
            }
            message = "Menu Item Added Successfully.";
            return true;
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
                if (!string.IsNullOrWhiteSpace(menuItemName) &&
                    !string.IsNullOrWhiteSpace(menuItem.Name) &&
                    menuItem.Name.Trim().Equals(menuItemName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return menuItem;
                }
            }
            return null;
        }

        public List<MenuItem> GetAllMenuItems()
        {
            return new List<MenuItem>(menuItems);
        }

        public Dictionary<int, double> CreateRecipe()
        {
            return new Dictionary<int, double>();
        }

        public void AddRecipeIngredient(Dictionary<int, double> recipeIngredients, int ingredientId, double requiredQuantity)
        {
            if (recipeIngredients.ContainsKey(ingredientId))
            {
                recipeIngredients[ingredientId] += requiredQuantity;
            }
            else
            {
                recipeIngredients.Add(ingredientId, requiredQuantity);
            }
        }

        private bool ValidateMenuItem(MenuItem menuItem, out string message)
        {
            if (menuItem is null)
            {
                message = "Menu item cannot be null.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(menuItem.Name))
            {
                message = "Menu item name cannot be empty.";
                return false;
            }
            if (menuItem.Price <= 0)
            {
                message = "Menu item price must be greater than zero.";
                return false;
            }
            if (menuItem.Recipe == null)
            {
                message = "Menu item recipe cannot be null.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private bool SaveMenu()
        {
            storageErrorMessage = string.Empty;
            try
            {
                JsonSerializerOptions jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string menuJson = JsonSerializer.Serialize(menuItems, jsonOptions);
                return FileManager.TryWriteAllText(menuFilePath, menuJson);
            }
            catch (Exception ex)
            {
                storageErrorMessage = $"Unexpected error while preparing menu data: {ex.Message}";
                return false;
            }
        }

        private void LoadMenu()
        {
            LoadMessage = string.Empty;
            if (!FileManager.TryReadAllText(menuFilePath, out string menuJson))
            {
                LoadMessage = FileManager.LastErrorMessage;
                return;
            }
            try
            {
                List<MenuItem?>? savedMenuItems = JsonSerializer.Deserialize<List<MenuItem?>>(menuJson);
                if (savedMenuItems != null)
                {
                    foreach (MenuItem? savedMenuItem in savedMenuItems)
                    {
                        if (savedMenuItem == null || !IsStoredMenuItemValid(savedMenuItem))
                        {
                            continue;
                        }
                        if (savedMenuItem.Recipe == null)
                        {
                            savedMenuItem.Recipe = new Dictionary<int, double>();
                        }
                        menuItems.Add(savedMenuItem);
                    }
                }
            }
            catch (JsonException ex)
            {
                menuItems.Clear();
                LoadMessage = $"Menu file contains invalid JSON and could not be loaded: {ex.Message}";
            }
            catch (Exception ex)
            {
                menuItems.Clear();
                LoadMessage = $"Unexpected error while loading menu: {ex.Message}";
            }
        }

        private static bool IsStoredMenuItemValid(MenuItem menuItem)
        {
            return menuItem.MenuItemId > 0 &&
                   !string.IsNullOrWhiteSpace(menuItem.Name) &&
                   menuItem.Price > 0;
        }

        private string GetStorageErrorMessage(string fallbackMessage)
        {
            if (!string.IsNullOrWhiteSpace(storageErrorMessage))
            {
                return storageErrorMessage;
            }
            if (!string.IsNullOrWhiteSpace(FileManager.LastErrorMessage))
            {
                return FileManager.LastErrorMessage;
            }
            return fallbackMessage;
        }
    }
}
