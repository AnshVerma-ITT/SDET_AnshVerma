using System.Text.Json;
using GourmetSpot.Exceptions;
using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class MenuManager : IStoreManager<MenuItem>
    {
        private List<MenuItem> menuItems;
        private string menuFilePath = FileManager.MenuFilePath;
        private string storageErrorMessage = string.Empty;

        public string LoadMessage { get; private set; } = string.Empty;

        public MenuManager()
        {
            menuItems = new List<MenuItem>();
            menuItems = Load();
        }

        public int GetNextMenuItemId()
        {
            int highestMenuItemId = 0;
            foreach (MenuItem menuItem in menuItems)
            {
                highestMenuItemId = Math.Max(highestMenuItemId, menuItem.MenuItemId);
            }
            return highestMenuItemId + 1;
        }

        public bool AddMenuItem(MenuItem menuItem, out string message)
        {
            if (!ValidateMenuItem(menuItem, out message))
            {
                return false;
            }
            menuItems.Add(menuItem);
            if (!Save(menuItems))
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

        public bool Save(List<MenuItem> items)
        {
            storageErrorMessage = string.Empty;
            try
            {
                JsonSerializerOptions jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string menuJson = JsonSerializer.Serialize(items, jsonOptions);
                return FileManager.TryWriteAllText(menuFilePath, menuJson);
            }
            catch (Exception ex)
            {
                storageErrorMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException("Unexpected error while preparing menu data.", ex));
                return false;
            }
        }

        public List<MenuItem> Load()
        {
            LoadMessage = string.Empty;
            if (!FileManager.TryReadAllText(menuFilePath, out string menuJson))
            {
                LoadMessage = FileManager.LastErrorMessage;
                return new List<MenuItem>();
            }
            try
            {
                List<MenuItem> loadedMenuItems = new List<MenuItem>();
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
                        loadedMenuItems.Add(savedMenuItem);
                    }
                }
                menuItems = loadedMenuItems;
                return loadedMenuItems;
            }
            catch (JsonException ex)
            {
                menuItems.Clear();
                LoadMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException("Menu file contains invalid JSON and could not be loaded.", ex));
                return new List<MenuItem>();
            }
            catch (Exception ex)
            {
                menuItems.Clear();
                LoadMessage = ExceptionUtilities.GetMessage(
                    new FileOperationException("Unexpected error while loading menu.", ex));
                return new List<MenuItem>();
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
