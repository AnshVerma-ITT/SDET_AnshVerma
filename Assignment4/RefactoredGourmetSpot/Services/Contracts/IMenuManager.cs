using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IMenuManager
    {
        string LoadMessage { get; }

        int GetNextMenuItemId();
        bool AddMenuItem(MenuItem menuItem, out string message);
        MenuItem SearchMenuItemById(int menuItemId);
        MenuItem SearchMenuItemByName(string menuItemName);
        List<MenuItem> GetAllMenuItems();
        Dictionary<int, double> CreateRecipe();
        void AddRecipeIngredient(Dictionary<int, double> recipeIngredients, int ingredientId, double requiredQuantity);
    }
}
