using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IInventoryManager
    {
        string LoadMessage { get; }

        int GetNextIngredientId();
        List<Ingredient> GetAllIngredients();
        bool AddIngredient(Ingredient ingredient, out string message);
        Ingredient SearchIngredientById(int ingredientId);
        Ingredient SearchIngredientByName(string ingredientName);
        bool AddIngredientQuantityByName(string ingredientName, double additionalQuantity, out string message);
        bool UpdateIngredientQuantityByName(string ingredientName, double newQuantity, out string message);
        bool DeleteIngredientByName(string ingredientName, out string message);
        bool HasEnoughIngredients(Dictionary<int, double> requiredIngredients, out string message);
        bool UseIngredients(Dictionary<int, double> requiredIngredients, out string message);
    }
}
