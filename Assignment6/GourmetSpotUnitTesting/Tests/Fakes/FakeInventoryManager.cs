using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;

namespace GourmetSpot.Tests.Fakes
{
    /// <summary>
    /// Minimal in-memory <see cref="IInventoryManager"/> double used to isolate
    /// <c>OrderManager</c> tests from the real <c>InventoryManager</c>.
    ///
    /// Only members that OrderManagerTests actually exercises
    /// (<see cref="SearchIngredientById"/>, <see cref="CalculateRequiredIngredients"/>,
    /// <see cref="UseIngredients"/>) are implemented. The rest intentionally
    /// throw <see cref="NotSupportedException"/> so this fake never grows a
    /// second, independently-maintained copy of InventoryManager's business
    /// rules (validation, stock checks, etc.) that the real unit tests for
    /// InventoryManager already cover.
    /// </summary>
    internal class FakeInventoryManager : IInventoryManager
    {
        private readonly List<Ingredient> ingredients;
        public string LoadMessage { get; private set; } = string.Empty;

        public FakeInventoryManager(List<Ingredient> ingredients)
        {
            this.ingredients = ingredients;
        }

        public Ingredient? SearchIngredientById(int ingredientId) =>
            ingredients.Find(ingredient => ingredient.IngredientId == ingredientId);

        public Dictionary<int, double> CalculateRequiredIngredients(List<OrderItem> selectedMenuItems)
        {
            Dictionary<int, double> requiredIngredients = new Dictionary<int, double>();
            foreach (OrderItem selectedMenuItem in selectedMenuItems)
            {
                foreach (var recipeIngredient in selectedMenuItem.MenuItem.Recipe)
                {
                    double requiredQuantity = recipeIngredient.Value * selectedMenuItem.Quantity;
                    requiredIngredients[recipeIngredient.Key] =
                        requiredIngredients.GetValueOrDefault(recipeIngredient.Key) + requiredQuantity;
                }
            }
            return requiredIngredients;
        }

        public bool UseIngredients(Dictionary<int, double> requiredIngredients, out string message)
        {
            foreach (var requiredIngredient in requiredIngredients)
            {
                SearchIngredientById(requiredIngredient.Key)!.Quantity -= requiredIngredient.Value;
            }
            message = string.Empty;
            return true;
        }

        public int GetNextIngredientId() =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(GetNextIngredientId)}.");

        public List<Ingredient> GetAllIngredients() =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(GetAllIngredients)}.");

        public bool AddIngredient(Ingredient ingredient, out string message) =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(AddIngredient)}.");

        public Ingredient? SearchIngredientByName(string ingredientName) =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(SearchIngredientByName)}.");

        public bool UpdateIngredientQuantityByName(string ingredientName, double newQuantity, out string message) =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(UpdateIngredientQuantityByName)}.");

        public bool DeleteIngredientByName(string ingredientName, out string message) =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(DeleteIngredientByName)}.");

        public bool HasEnoughIngredients(Dictionary<int, double> requiredIngredients, out string message) =>
            throw new NotSupportedException($"{nameof(FakeInventoryManager)} does not support {nameof(HasEnoughIngredients)}.");
    }
}
