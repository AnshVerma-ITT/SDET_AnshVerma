using GourmetSpot.Models;

namespace GourmetSpot.Services.Contracts
{
    public interface IIngredientRequirementCalculator
    {
        Dictionary<int, double> CalculateRequiredIngredients(List<OrderItemSelection> selectedMenuItems);
    }
}
