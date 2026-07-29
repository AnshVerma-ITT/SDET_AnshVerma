using GourmetSpot.Models;
using GourmetSpot.Services;
using GourmetSpot.Utilities;

namespace GourmetSpot.UserInterface
{
    public class InventoryScreen
    {
        private InventoryManager inventoryManager;

        public InventoryScreen(InventoryManager inventoryManager)
        {
            this.inventoryManager = inventoryManager;
        }

        public void Show()
        {
            while (true)
            {
                DisplayInventoryMenu();
                string userChoice = ConsoleInput.ReadMenuChoice();
                switch (userChoice)
                {
                    case "1":
                        ScreenActionRunner.TryRun(AddIngredient);
                        break;
                    case "2":
                        ScreenActionRunner.TryRun(DisplayInventory);
                        break;
                    case "3":
                        ScreenActionRunner.TryRun(SearchIngredientByName);
                        break;
                    case "4":
                        ScreenActionRunner.TryRun(UpdateIngredientByName);
                        break;
                    case "5":
                        ScreenActionRunner.TryRun(DeleteIngredientByName);
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void DisplayInventoryMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== Inventory Menu =====");
            Console.WriteLine("1. Add Ingredient");
            Console.WriteLine("2. View Ingredient");
            Console.WriteLine("3. Search Ingredient by Name");
            Console.WriteLine("4. Update Ingredient by Name");
            Console.WriteLine("5. Delete Ingredient by Name");
            Console.WriteLine("6. Back");
            Console.Write("Enter your choice: ");
        }

        private void AddIngredient()
        {
            string ingredientName = ConsoleInput.ReadRequiredText("Enter Ingredient Name: ");
            Ingredient? existingIngredient = inventoryManager.SearchIngredientByName(ingredientName);
            if (existingIngredient != null)
            {
                Console.WriteLine("\nIngredient already exists in stock.");
                Console.WriteLine("-------------------------");
                DisplayIngredient(existingIngredient);
                bool shouldAddStock = ConsoleInput.ReadYesNo("Do you want to add more quantity to this ingredient? (y/n): ");
                if (shouldAddStock)
                {
                    double additionalQuantity = ConsoleInput.ReadNonNegativeDouble("Enter Quantity to Add: ");
                    inventoryManager.AddIngredientQuantityByName(existingIngredient.Name, additionalQuantity, out string updateMessage);
                    Console.WriteLine(updateMessage);
                }
                else
                {
                    Console.WriteLine("New ingredient was not added.");
                }
                return;
            }
            int ingredientId = inventoryManager.GetNextIngredientId();
            Console.WriteLine($"Ingredient ID: {ingredientId}");
            double ingredientQuantity = ConsoleInput.ReadNonNegativeDouble("Enter Quantity: ");
            string ingredientUnit = ConsoleInput.ReadRequiredText("Enter Unit: ");
            Ingredient ingredient = new Ingredient(
                ingredientId,
                ingredientName,
                ingredientQuantity,
                ingredientUnit);
            inventoryManager.AddIngredient(ingredient, out string addMessage);
            Console.WriteLine(addMessage);
        }

        private void DisplayInventory()
        {
            List<Ingredient> ingredients = inventoryManager.GetAllIngredients();
            if (ingredients.Count == 0)
            {
                Console.WriteLine("\nInventory is empty.");
                return;
            }
            Console.WriteLine("\n========== INVENTORY ==========");
            foreach (Ingredient ingredient in ingredients)
            {
                DisplayIngredient(ingredient);
            }
        }

        private void SearchIngredientByName()
        {
            string ingredientName = ConsoleInput.ReadRequiredText("Enter Ingredient Name: ");
            Ingredient? ingredient = inventoryManager.SearchIngredientByName(ingredientName);
            if (ingredient != null)
            {
                Console.WriteLine("\nIngredient Found");
                Console.WriteLine("-------------------------");
                DisplayIngredient(ingredient);
            }
            else
            {
                Console.WriteLine("Ingredient not found.");
            }
        }

        private void UpdateIngredientByName()
        {
            string ingredientName = ConsoleInput.ReadRequiredText("Enter Ingredient Name: ");
            Ingredient? ingredient = inventoryManager.SearchIngredientByName(ingredientName);
            if (ingredient == null)
            {
                Console.WriteLine("Ingredient not found.");
                return;
            }
            Console.WriteLine($"Current Stock: {ingredient.Quantity} {ingredient.Unit}");
            double newQuantity = ConsoleInput.ReadNonNegativeDouble("Enter New Quantity: ");
            inventoryManager.UpdateIngredientQuantityByName(ingredientName, newQuantity, out string updateMessage);
            Console.WriteLine(updateMessage);
        }

        private void DeleteIngredientByName()
        {
            string ingredientName = ConsoleInput.ReadRequiredText("Enter Ingredient Name: ");
            inventoryManager.DeleteIngredientByName(ingredientName, out string deleteMessage);
            Console.WriteLine(deleteMessage);
        }

        private void DisplayIngredient(Ingredient ingredient)
        {
            Console.WriteLine($"{ingredient.IngredientId} - {ingredient.Name} - {ingredient.Quantity} {ingredient.Unit}");
        }
    }
}
