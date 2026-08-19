using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;

namespace GourmetSpot.Tests.Fakes
{
    internal class FakeInventoryManager : IInventoryManager
    {
        private readonly List<Ingredient> ingredients;
        public string LoadMessage { get; private set; } = string.Empty;

        public FakeInventoryManager(List<Ingredient> ingredients)
        {
            this.ingredients = ingredients;
        }

        public int GetNextIngredientId()
        {
            return ingredients.Count + 1;
        }

        public List<Ingredient> GetAllIngredients()
        {
            return new List<Ingredient>(ingredients);
        }

        public bool AddIngredient(Ingredient ingredient, out string message)
        {
            ingredients.Add(ingredient);
            message = string.Empty;
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
                if (ingredient.Name.Equals(ingredientName, StringComparison.OrdinalIgnoreCase))
                {
                    return ingredient;
                }
            }
            return null;
        }

        public bool UpdateIngredientQuantityByName(
            string ingredientName,
            double newQuantity,
            out string message)
        {
            message = string.Empty;
            return false;
        }

        public bool DeleteIngredientByName(string ingredientName, out string message)
        {
            message = string.Empty;
            return false;
        }

        public Dictionary<int, double> CalculateRequiredIngredients(List<OrderItem> selectedMenuItems)
        {
            Dictionary<int, double> requiredIngredients = new Dictionary<int, double>();
            foreach (OrderItem selectedMenuItem in selectedMenuItems)
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

        public bool HasEnoughIngredients(
            Dictionary<int, double> requiredIngredients,
            out string message)
        {
            foreach (var requiredIngredient in requiredIngredients)
            {
                Ingredient? ingredient = SearchIngredientById(requiredIngredient.Key);
                if (ingredient == null || ingredient.Quantity < requiredIngredient.Value)
                {
                    message = "Insufficient stock.";
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        public bool UseIngredients(
            Dictionary<int, double> requiredIngredients,
            out string message)
        {
            if (!HasEnoughIngredients(requiredIngredients, out message))
            {
                return false;
            }
            foreach (var requiredIngredient in requiredIngredients)
            {
                SearchIngredientById(requiredIngredient.Key)!.Quantity -= requiredIngredient.Value;
            }
            message = string.Empty;
            return true;
        }
    }
}
