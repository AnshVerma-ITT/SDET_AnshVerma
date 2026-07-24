using GourmetSpot.Models;

namespace GourmetSpot.Services
{
    public class InventoryManager
    {
        private readonly List<Ingredient> inventory;
        private readonly string filePath = "Data/inventory.txt";

        public InventoryManager()
        {
            inventory = new List<Ingredient>();

            Directory.CreateDirectory("Data");

            LoadInventory();
        }

        public void AddIngredient(Ingredient ingredient)
        {
            inventory.Add(ingredient);
            SaveInventory();
            Console.WriteLine("Ingredient added successfully.");
        }

        public void DisplayInventory()
        {
            if (inventory.Count == 0)
            {
                Console.WriteLine("\nInventory is empty.");
                return;
            }

            Console.WriteLine("\n========== INVENTORY ==========");

            foreach (Ingredient ingredient in inventory)
            {
                Console.WriteLine(ingredient);
            }
        }

        public Ingredient? SearchIngredientById(int id)
        {
            foreach (Ingredient ingredient in inventory)
            {
                if (ingredient.IngredientId == id)
                    return ingredient;
            }

            return null;
        }

        public bool UpdateIngredientQuantity(int id, double quantity)
        {
            Ingredient? ingredient = SearchIngredientById(id);

            if (ingredient == null)
                return false;

            ingredient.Quantity = quantity;

            SaveInventory();

            return true;
        }

        public bool DeleteIngredient(int id)
        {
            Ingredient? ingredient = SearchIngredientById(id);

            if (ingredient == null)
                return false;

            inventory.Remove(ingredient);

            SaveInventory();

            return true;
        }

        private void SaveInventory()
        {
            List<string> lines = new List<string>();

            foreach (Ingredient ingredient in inventory)
            {
                lines.Add($"{ingredient.IngredientId},{ingredient.Name},{ingredient.Quantity},{ingredient.Unit}");
            }

            File.WriteAllLines(filePath, lines);
        }

        private void LoadInventory()
        {
            if (!File.Exists(filePath))
                return;

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] data = line.Split(',');

                Ingredient ingredient = new Ingredient(
                    Convert.ToInt32(data[0]),
                    data[1],
                    Convert.ToDouble(data[2]),
                    data[3]);

                inventory.Add(ingredient);
            }
        }
        public bool HasEnoughIngredients(Dictionary<int, double> recipe, int orderQuantity)
        {
            foreach (var item in recipe)
            {
                Ingredient? ingredient = SearchIngredientById(item.Key);
                if (ingredient == null)
                {
                    Console.WriteLine($"Ingredient ID {item.Key} not found.");
                    return false;
                }

                double requiredQuantity = item.Value * orderQuantity;

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
            if (!HasEnoughIngredients(recipe, orderQuantity))
                return false;

            foreach (var item in recipe)
            {
                Ingredient ingredient = SearchIngredientById(item.Key)!;
                ingredient.Quantity -= item.Value * orderQuantity;
            }

            SaveInventory();
            return true;
        }
    }
    
}