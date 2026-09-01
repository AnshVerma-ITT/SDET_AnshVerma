namespace GourmetSpot.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }

        public Ingredient(int ingredientId, string name, double quantity, string unit)
        {
            IngredientId = ingredientId;
            Name = name;
            Quantity = quantity;
            Unit = unit;
        }
    }
}
