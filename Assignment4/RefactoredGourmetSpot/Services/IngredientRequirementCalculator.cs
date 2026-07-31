using GourmetSpot.Models;
using GourmetSpot.Services.Contracts;

namespace GourmetSpot.Services
{
    public class IngredientRequirementCalculator : IIngredientRequirementCalculator
    {
        public Dictionary<int, double> CalculateRequiredIngredients(List<OrderItemSelection> selectedMenuItems)
        {
            Dictionary<int, double> requiredIngredients = new Dictionary<int, double>();
            foreach (OrderItemSelection selectedMenuItem in selectedMenuItems)
            {
                if (selectedMenuItem.MenuItem.Recipe == null)
                {
                    continue;
                }
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
    }
}
