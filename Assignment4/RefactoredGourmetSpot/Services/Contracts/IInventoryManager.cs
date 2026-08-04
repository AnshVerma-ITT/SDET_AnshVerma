using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IInventoryManager
    {
        string LoadMessage { get; }

        int GetNextIngredientId();
        List<Ingredient> GetAllIngredients();
        bool AddIngredient(Ingredient ingredient, out string message);
        Ingredient? SearchIngredientById(int ingredientId);
        Ingredient? SearchIngredientByName(string ingredientName);
        bool UpdateIngredientQuantityByName(string ingredientName, double newQuantity, out string message);
        bool DeleteIngredientByName(string ingredientName, out string message);
        Dictionary<int, double> CalculateRequiredIngredients(List<OrderItem> selectedMenuItems);
        bool HasEnoughIngredients(Dictionary<int, double> requiredIngredients, out string message);
        bool UseIngredients(Dictionary<int, double> requiredIngredients, out string message);
    }
}
