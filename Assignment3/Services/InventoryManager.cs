using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class InventoryManager
    {
        private readonly List<Ingredient> ingredients;
        private readonly string inventoryFilePath = ApplicationStorage.InventoryFilePath;

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

        public bool AddIngredient(Ingredient ingredient)
        {
            Ingredient? existingIngredient = SearchIngredientByName(ingredient.Name);

            if (existingIngredient != null)
            {
                Console.WriteLine("Ingredient already exists in inventory.");
                return false;
            }

            ingredients.Add(ingredient);
            SaveInventory();
            Console.WriteLine("Ingredient added successfully.");
            return true;
        }

        public void DisplayInventory()
        {
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

        public bool AddIngredientQuantityByName(string ingredientName, double additionalQuantity)
        {
            Ingredient? ingredient = SearchIngredientByName(ingredientName);

            if (ingredient == null)
            {
                return false;
            }

            ingredient.Quantity += additionalQuantity;
            SaveInventory();
            return true;
        }

        public bool UpdateIngredientQuantityByName(string ingredientName, double newQuantity)
        {
            Ingredient? ingredient = SearchIngredientByName(ingredientName);

            if (ingredient == null)
            {
                return false;
            }

            ingredient.Quantity = newQuantity;
            SaveInventory();
            return true;
        }

        public bool DeleteIngredientByName(string ingredientName)
        {
            Ingredient? ingredient = SearchIngredientByName(ingredientName);

            if (ingredient == null)
            {
                return false;
            }

            ingredients.Remove(ingredient);
            SaveInventory();
            return true;
        }

        public bool HasEnoughIngredients(Dictionary<int, double> recipe, int orderQuantity)
        {
            Dictionary<int, double> requiredIngredients = CalculateRequiredIngredients(recipe, orderQuantity);
            return HasEnoughIngredients(requiredIngredients);
        }

        public bool HasEnoughIngredients(Dictionary<int, double> requiredIngredients)
        {
            foreach (var requiredIngredient in requiredIngredients)
            {
                Ingredient? ingredient = SearchIngredientById(requiredIngredient.Key);

                if (ingredient == null)
                {
                    Console.WriteLine($"Ingredient ID {requiredIngredient.Key} not found.");
                    return false;
                }

                double requiredQuantity = requiredIngredient.Value;

                if (ingredient.Quantity < requiredQuantity)
                {
                    Console.WriteLine($"Insufficient stock for {ingredient.Name}");
                    return false;
                }
            }

            return true;
        }

        public bool UseIngredients(Dictionary<int, double> recipe, int orderQuantity)
        {
            Dictionary<int, double> requiredIngredients = CalculateRequiredIngredients(recipe, orderQuantity);
            return UseIngredients(requiredIngredients);
        }

        public bool UseIngredients(Dictionary<int, double> requiredIngredients)
        {
            if (!HasEnoughIngredients(requiredIngredients))
            {
                return false;
            }

            foreach (var requiredIngredient in requiredIngredients)
            {
                Ingredient ingredient = SearchIngredientById(requiredIngredient.Key)!;
                ingredient.Quantity -= requiredIngredient.Value;
            }

            SaveInventory();
            return true;
        }

        private static bool IngredientNameMatches(Ingredient ingredient, string ingredientName)
        {
            return ingredient.Name.Trim().Equals(ingredientName.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private void SaveInventory()
        {
            List<string> inventoryLines = new List<string>();

            foreach (Ingredient ingredient in ingredients)
            {
                inventoryLines.Add($"{ingredient.IngredientId},{ingredient.Name},{ingredient.Quantity},{ingredient.Unit}");
            }

            ApplicationStorage.TryWriteAllLines(inventoryFilePath, inventoryLines);
        }

        private void LoadInventory()
        {
            if (!ApplicationStorage.TryReadAllLines(inventoryFilePath, out string[] inventoryLines))
            {
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

        private Dictionary<int, double> CalculateRequiredIngredients(Dictionary<int, double> recipe, int orderQuantity)
        {
            Dictionary<int, double> requiredIngredients = new Dictionary<int, double>();

            foreach (var recipeIngredient in recipe)
            {
                requiredIngredients[recipeIngredient.Key] = recipeIngredient.Value * orderQuantity;
            }

            return requiredIngredients;
        }
    }
}
