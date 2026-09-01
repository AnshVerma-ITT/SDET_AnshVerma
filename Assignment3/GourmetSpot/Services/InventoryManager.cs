using GourmetSpot.Models;
using GourmetSpot.Utilities;

namespace GourmetSpot.Services
{
    public class InventoryManager
    {
        private List<Ingredient> ingredients;
        private string inventoryFilePath = FileManager.InventoryFilePath;

        public string LoadMessage { get; private set; } = string.Empty;

        public InventoryManager()
        {
            ingredients = new List<Ingredient>();
            LoadInventory();
        }

        public int GetNextIngredientId()
        {
            int nextIngredientId = 1;
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.IngredientId >= nextIngredientId)
                {
                    nextIngredientId = ingredient.IngredientId + 1;
                }
            }
            return nextIngredientId;
        }

        public List<Ingredient> GetAllIngredients()
        {
            return new List<Ingredient>(ingredients);
        }

        public bool AddIngredient(Ingredient ingredient, out string message)
        {
            if (!ValidateIngredient(ingredient, out message))
            {
                return false;
            }
            Ingredient? existingIngredient = SearchIngredientByName(ingredient.Name);
            if (existingIngredient != null)
            {
                message = "Ingredient already exists in inventory.";
                return false;
            }
            ingredients.Add(ingredient);
            if (!SaveInventory())
            {
                message = GetStorageErrorMessage("Ingredient added, but inventory could not be saved.");
                return false;
            }
            message = "Ingredient added successfully.";
            return true;
        }

        public Ingredient? SearchIngredientById(int ingredientId)
        {
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.IngredientId == ingredientId)
                {
                    return ingredient;
                }
            }
            return null;
        }

        public Ingredient? SearchIngredientByName(string ingredientName)
        {
            foreach (Ingredient ingredient in ingredients)
            {
                if (IngredientNameMatches(ingredient, ingredientName))
                {
                    return ingredient;
                }
            }
            return null;
        }

        public bool AddIngredientQuantityByName(string ingredientName, double additionalQuantity, out string message)
        {
            Ingredient? ingredient = SearchIngredientByName(ingredientName);
            if (ingredient == null)
            {
                message = "Ingredient not found.";
                return false;
            }
            if (additionalQuantity < 0)
            {
                message = "Quantity cannot be negative.";
                return false;
            }
            ingredient.Quantity += additionalQuantity;
            if (!SaveInventory())
            {
                message = GetStorageErrorMessage("Ingredient quantity updated, but inventory could not be saved.");
                return false;
            }
            message = "Ingredient quantity updated successfully.";
            return true;
        }

        public bool UpdateIngredientQuantityByName(string ingredientName, double newQuantity, out string message)
        {
            Ingredient? ingredient = SearchIngredientByName(ingredientName);
            if (ingredient == null)
            {
                message = "Ingredient not found.";
                return false;
            }
            if (newQuantity < 0)
            {
                message = "Quantity cannot be negative.";
                return false;
            }
            ingredient.Quantity += newQuantity;
            if (!SaveInventory())
            {
                message = GetStorageErrorMessage("Ingredient updated, but inventory could not be saved.");
                return false;
            }
            message = "Ingredient updated successfully.";
            return true;
        }

        public bool DeleteIngredientByName(string ingredientName, out string message)
        {
            Ingredient? ingredient = SearchIngredientByName(ingredientName);
            if (ingredient == null)
            {
                message = "Ingredient not found.";
                return false;
            }
            ingredients.Remove(ingredient);
            if (!SaveInventory())
            {
                message = GetStorageErrorMessage("Ingredient deleted, but inventory could not be saved.");
                return false;
            }
            message = "Ingredient deleted successfully.";
            return true;
        }

        public bool HasEnoughIngredients(Dictionary<int, double> requiredIngredients, out string message)
        {
            foreach (var requiredIngredient in requiredIngredients)
            {
                Ingredient? ingredient = SearchIngredientById(requiredIngredient.Key);
                if (ingredient == null)
                {
                    message = $"Ingredient ID {requiredIngredient.Key} not found.";
                    return false;
                }
                double requiredQuantity = requiredIngredient.Value;
                if (ingredient.Quantity < requiredQuantity)
                {
                    message = $"Insufficient stock for {ingredient.Name}";
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        public bool UseIngredients(Dictionary<int, double> requiredIngredients, out string message)
        {
            if (!HasEnoughIngredients(requiredIngredients, out message))
            {
                return false;
            }
            foreach (var requiredIngredient in requiredIngredients)
            {
                Ingredient ingredient = SearchIngredientById(requiredIngredient.Key)!;
                ingredient.Quantity -= requiredIngredient.Value;
            }
            if (!SaveInventory())
            {
                message = GetStorageErrorMessage("Inventory was updated, but it could not be saved.");
                return false;
            }
            message = string.Empty;
            return true;
        }

        private bool ValidateIngredient(Ingredient ingredient, out string validationMessage)
        {
            if (ingredient is null)
            {
                validationMessage = "Ingredient cannot be null.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ingredient.Name))
            {
                validationMessage = "Ingredient name cannot be empty.";
                return false;
            }
            if (ingredient.Quantity < 0)
            {
                validationMessage = "Quantity cannot be negative.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(ingredient.Unit))
            {
                validationMessage = "Ingredient unit cannot be empty.";
                return false;
            }
            validationMessage = string.Empty;
            return true;
        }

        private static bool IngredientNameMatches(Ingredient ingredient, string ingredientName)
        {
            return !string.IsNullOrWhiteSpace(ingredient.Name) &&
                   !string.IsNullOrWhiteSpace(ingredientName) &&
                   ingredient.Name.Trim().Equals(ingredientName.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private bool SaveInventory()
        {
            List<string> inventoryLines = new List<string>();
            foreach (Ingredient ingredient in ingredients)
            {
                string ingredientName = (ingredient.Name ?? "").Replace(",", " ");
                string ingredientUnit = (ingredient.Unit ?? "").Replace(",", " ");
                inventoryLines.Add($"{ingredient.IngredientId},{ingredientName},{ingredient.Quantity},{ingredientUnit}");
            }
            return FileManager.TryWriteAllLines(inventoryFilePath, inventoryLines);
        }

        private void LoadInventory()
        {
            LoadMessage = string.Empty;
            if (!FileManager.TryReadAllLines(inventoryFilePath, out string[] inventoryLines))
            {
                LoadMessage = FileManager.LastErrorMessage;
                return;
            }
            foreach (string inventoryLine in inventoryLines)
            {
                string[] ingredientData = inventoryLine.Split(',');
                if (ingredientData.Length < 4)
                {
                    continue;
                }
                bool ingredientIdValid = int.TryParse(ingredientData[0], out int ingredientId);
                bool ingredientQuantityValid = double.TryParse(ingredientData[2], out double ingredientQuantity);
                if (!ingredientIdValid || !ingredientQuantityValid)
                {
                    continue;
                }
                Ingredient ingredient = new Ingredient(
                    ingredientId,
                    ingredientData[1],
                    ingredientQuantity,
                    ingredientData[3]);
                ingredients.Add(ingredient);
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
